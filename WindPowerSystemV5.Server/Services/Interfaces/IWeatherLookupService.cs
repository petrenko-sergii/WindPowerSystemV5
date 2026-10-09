using WindPowerSystemV5.Server.Data.DTOs;

namespace WindPowerSystemV5.Server.Services.Interfaces;

public interface IWeatherLookupService
{
    Task<CurrentWeatherDTO> GetCurrentWeather(
        string city, decimal? lat = null, decimal? lon = null, CancellationToken cancellationToken = default);
}
