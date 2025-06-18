using Microsoft.Azure.Cosmos;

namespace WindPowerSystemV5.Server.Services.Interfaces;

public interface ICosmosDbContext
{
    Container MaintenanceRecordsContainer { get; }

    Container TurbineCharacteristicsContainer { get; }

    Container TurbineConfigSnapshotsContainer { get; }
}
