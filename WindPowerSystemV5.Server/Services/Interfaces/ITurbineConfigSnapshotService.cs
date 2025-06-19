using WindPowerSystemV5.Server.Data.DTOs;

namespace WindPowerSystemV5.Server.Services.Interfaces;

public interface ITurbineConfigSnapshotService
{
    Task<TurbineConfigSnapshotDTO?> Get(string id);

    Task<List<TurbineConfigSnapshotDTO>> GetByOperatingMode(string operatingMode);

    Task<List<TurbineConfigSnapshotDTO>> GetByTurbineId(int turbineId);

    Task Update(string id, TurbineConfigSnapshotDTO recordToUpdate);
}
