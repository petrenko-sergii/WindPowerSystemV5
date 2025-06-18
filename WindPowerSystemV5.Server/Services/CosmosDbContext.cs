using Microsoft.Azure.Cosmos;
using WindPowerSystemV5.Server.Services.Interfaces;

namespace WindPowerSystemV5.Server.Services;

public class CosmosDbContext : ICosmosDbContext
{
    private readonly CosmosClient _client;
    private readonly string _databaseName;

    public CosmosDbContext(IConfiguration configuration)
    {
        var cosmosDbEndpoint = configuration["CosmosDb:Endpoint"];
        var cosmosDbKey = configuration["CosmosDb:PrimaryKey"];
        _databaseName = configuration["CosmosDb:Database"] 
            ?? throw new ArgumentException($"The configuration \"CosmosDb:Database\" was not provided.");

        _client = new CosmosClient(cosmosDbEndpoint, cosmosDbKey);
    }

    public Container MaintenanceRecordsContainer =>
        _client.GetContainer(_databaseName, ContainerNames.MaintenanceRecords);

    public Container TurbineCharacteristicsContainer =>
      _client.GetContainer(_databaseName, ContainerNames.TurbineCharacteristics);

    public Container TurbineConfigSnapshotsContainer => 
        _client.GetContainer(_databaseName, ContainerNames.TurbineConfigSnapshots);
}
