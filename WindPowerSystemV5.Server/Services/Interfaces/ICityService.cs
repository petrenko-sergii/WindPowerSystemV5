using WindPowerSystemV5.Server.Data.DTOs;

namespace WindPowerSystemV5.Server.Services.Interfaces;

public interface ICityService
{
    Task<List<CityDTO>> Get();

    Task<List<CityDTO>> GetWithCountry();

    Task<CityDTO> GetCached(int id);
}
