using WindPowerSystemV5.Server.Data.Models;

namespace WindPowerSystemV5.Server.Data.Repositories.Interfaces;

public interface ICityRepository
{
    Task<List<City>> Get();

    Task<List<City>> GetWithCountry();

    Task<City?> Create(City city);
    
    Task<City?> GetCached(int id);
}
