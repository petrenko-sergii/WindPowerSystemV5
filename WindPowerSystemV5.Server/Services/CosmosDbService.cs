using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using WindPowerSystemV5.Server.Data.NoSqlModels;

namespace WindPowerSystemV5.Server.Services;

public class CosmosDbService
{
    private readonly Container _container;

    public CosmosDbService(CosmosClient client, string databaseName, string containerName)
    {
        _container = client.GetContainer(databaseName, containerName);
    }

    public async Task<IEnumerable<MaintenanceRecord>> GetRecordsByTurbineAsync(int turbineId)
    {
        var query = _container.GetItemLinqQueryable<MaintenanceRecord>(allowSynchronousQueryExecution: false)
            .Where(r => r.TurbineId == turbineId)
            .ToFeedIterator();

        List<MaintenanceRecord> results = new();
        while (query.HasMoreResults)
        {
            var response = await query.ReadNextAsync();
            results.AddRange(response);
        }

        return results;
    }
}
