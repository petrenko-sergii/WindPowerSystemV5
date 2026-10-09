using WindPowerSystemV5.Server.Data.DTOs;

namespace WindPowerSystemV5.Server.Services.Interfaces;

public interface IWeatherAdvisorService
{
    Task<WeatherClothingDTO> GetClothingAdvice(
        string city, decimal? lat = null, decimal? lon = null, CancellationToken cancellationToken = default);
}
