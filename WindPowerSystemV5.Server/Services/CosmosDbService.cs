using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System.Net;
using WindPowerSystemV5.Server.Data.NoSqlModels;
using WindPowerSystemV5.Server.Utils.Exceptions;

namespace WindPowerSystemV5.Server.Services;

public class CosmosDbService
{
    private readonly Container _container;

    public CosmosDbService(CosmosClient client, string databaseName, string containerName)
    {
        _container = client.GetContainer(databaseName, containerName);
    }

    public async Task<List<MaintenanceRecord>> GetMaintenanceRecordsByTurbineIdAsync(int turbineId)
    {
        var query = _container.GetItemLinqQueryable<MaintenanceRecord>(allowSynchronousQueryExecution: false)
            .Where(r => r.TurbineId == turbineId)
            .ToFeedIterator();

        List<MaintenanceRecord> results = [];
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results.ToList();
    }

    public async Task<MaintenanceRecord?> GetMaintenanceRecordAsync(string id)
    {
        try
        {
            var record = await _container.ReadItemAsync<MaintenanceRecord>(id, new PartitionKey(id));

            return record.Resource;
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            throw new NotFoundException($"MaintenanceRecord with ID {id} is not found.");
        }
    }
}
