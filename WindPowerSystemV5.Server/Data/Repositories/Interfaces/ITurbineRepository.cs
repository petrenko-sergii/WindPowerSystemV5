using WindPowerSystemV5.Server.Data.Models;

namespace WindPowerSystemV5.Server.Data.Repositories.Interfaces;

public interface ITurbineRepository
{
    Task<List<Turbine>> Get();

    Task<Turbine?> Get(int id);

    Task<int> Create(Turbine turbine);

    Task Update(Turbine turbine);
}
