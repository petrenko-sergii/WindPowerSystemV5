using WindPowerSystemV5.Server.Data.Models;

namespace WindPowerSystemV5.Server.Data.Repositories.Interfaces;

public interface ITurbineTypeRepository
{
    Task<int> Create(TurbineType turbineType);
}