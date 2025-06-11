using WindPowerSystemV5.Server.Data.NoSqlModels;

namespace WindPowerSystemV5.Server.Services.Interfaces;

public interface IMaintenanceRecordService
{
    Task<MaintenanceRecord?> Get(string id);

    Task<List<MaintenanceRecord>> GetByTurbineId(int turbineId);
}
