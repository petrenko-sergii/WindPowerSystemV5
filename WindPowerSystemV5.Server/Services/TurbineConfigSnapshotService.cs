using AutoMapper;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Data.NoSqlModels;
using WindPowerSystemV5.Server.Services.Interfaces;
using WindPowerSystemV5.Server.Utils.Exceptions;

namespace WindPowerSystemV5.Server.Services;

public class TurbineConfigSnapshotService : ITurbineConfigSnapshotService
{
    private readonly IMapper _mapper;
    private readonly ICosmosDbContext _cosmosDbContext;
    private readonly Container _turbineConfigSnapshotsContainer;

    public TurbineConfigSnapshotService(IMapper mapper, ICosmosDbContext cosmosDbContext)
    {
        _mapper = mapper;
        _cosmosDbContext = cosmosDbContext;
        _turbineConfigSnapshotsContainer = _cosmosDbContext.TurbineConfigSnapshotsContainer;
    }

    public async Task<TurbineConfigSnapshotDTO?> Get(string id)
    {
        TurbineConfigSnapshot? configSnapshot = await GetConfigSnapshot(id);

        return _mapper.Map<TurbineConfigSnapshotDTO>(configSnapshot);
    }

    public async Task<List<TurbineConfigSnapshotDTO>> GetByOperatingMode(string operatingMode)
    {
        var query = new QueryDefinition(
            "SELECT * FROM c WHERE c.OperatingMode = @operatingMode")
            .WithParameter("@operatingMode", operatingMode);

        var iterator = _turbineConfigSnapshotsContainer.GetItemQueryIterator<TurbineConfigSnapshot>(query);

        List<TurbineConfigSnapshot> results = [];
        while (iterator.HasMoreResults)
        {
            var response = await iterator.ReadNextAsync();
            results.AddRange(response);
        }

        return results.Select(r => _mapper.Map<TurbineConfigSnapshotDTO>(r)).ToList();
    }

    public async Task<List<TurbineConfigSnapshotDTO>> GetByTurbineId(int turbineId)
    {
        var query = _turbineConfigSnapshotsContainer
            .GetItemLinqQueryable<TurbineConfigSnapshot>(allowSynchronousQueryExecution: false)
            .Where(r => r.TurbineId == turbineId)
            .ToFeedIterator();

        List<TurbineConfigSnapshot> results = [];
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results.Select(r => _mapper.Map<TurbineConfigSnapshotDTO>(r)).ToList();
    }

    public async Task Update(string id, TurbineConfigSnapshotDTO recordToUpdate)
    {
        TurbineConfigSnapshot? existingRecord = await GetConfigSnapshot(id);

        if (existingRecord!.TurbineId == recordToUpdate.TurbineId)
        {
            _mapper.Map(recordToUpdate, existingRecord);
            await _turbineConfigSnapshotsContainer.ReplaceItemAsync(existingRecord, id, new PartitionKey(existingRecord.TurbineId));
            return;
        }

        var newRecord = _mapper.Map<TurbineConfigSnapshot>(recordToUpdate);

        await _turbineConfigSnapshotsContainer.CreateItemAsync(newRecord, new PartitionKey(newRecord.TurbineId));
        await _turbineConfigSnapshotsContainer.DeleteItemAsync<TurbineConfigSnapshot>(id, new PartitionKey(existingRecord.TurbineId));
    }

    private async Task<TurbineConfigSnapshot?> GetConfigSnapshot(string id)
    {
        var query = _turbineConfigSnapshotsContainer
           .GetItemLinqQueryable<TurbineConfigSnapshot>(allowSynchronousQueryExecution: false)
           .Where(x => x.Id == id)
           .ToFeedIterator();

        TurbineConfigSnapshot? existingRecord = null;
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            existingRecord = response.FirstOrDefault();
        }

        if (existingRecord == null)
        {
            throw new NotFoundException($"TurbineConfigSnapshot with ID {id} is not found.");
        }

        return existingRecord;
    }
}
