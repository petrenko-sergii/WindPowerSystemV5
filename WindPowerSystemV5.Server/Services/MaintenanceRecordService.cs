using AutoMapper;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Net;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Data.NoSqlModels;
using WindPowerSystemV5.Server.Services.Interfaces;
using WindPowerSystemV5.Server.Utils.Exceptions;

namespace WindPowerSystemV5.Server.Services;

public class MaintenanceRecordService : IMaintenanceRecordService
{
    private readonly IMapper _mapper;
    private readonly ICosmosDbContext _cosmosDbContext;
    private readonly Container _maintenanceRecordsContainer;

    public MaintenanceRecordService(IMapper mapper, ICosmosDbContext cosmosDbContext)
    {
        _mapper = mapper;
        _cosmosDbContext = cosmosDbContext;
        _maintenanceRecordsContainer = _cosmosDbContext.MaintenanceRecordsContainer;
    }

    public async Task<MaintenanceRecord?> Get(string id)
    {
        try
        {
            var record = await _maintenanceRecordsContainer
                .ReadItemAsync<MaintenanceRecord>(id, new PartitionKey(id));

            return record.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            throw new NotFoundException($"MaintenanceRecord with ID {id} is not found.");
        }
    }

    public async Task<List<MaintenanceRecord>> GetByTurbineId(int turbineId)
    {
        var query = _maintenanceRecordsContainer
            .GetItemLinqQueryable<MaintenanceRecord>(allowSynchronousQueryExecution: false)
            .Where(r => r.TurbineId == turbineId)
            .ToFeedIterator();

        List<MaintenanceRecord> results = [];
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return [.. results];
    }

    public async Task Update(string id, MaintenanceRecordDTO recordToUpdate)
    {
        MaintenanceRecord? existingRecord;

        try
        {
            var response = await _maintenanceRecordsContainer
                .ReadItemAsync<MaintenanceRecord>(id, new PartitionKey(id));
            existingRecord = response.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            throw new NotFoundException($"MaintenanceRecord with ID {id} is not found.");
        }

        _mapper.Map(recordToUpdate, existingRecord);

        await _maintenanceRecordsContainer.ReplaceItemAsync(existingRecord, id, new PartitionKey(id));
    }
}
