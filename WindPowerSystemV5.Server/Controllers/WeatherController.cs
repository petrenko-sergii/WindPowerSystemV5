using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Services.Interfaces;

namespace WindPowerSystemV5.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WeatherController : ControllerBase
{
    private readonly IWeatherAdvisorService _weatherAdvisorService;

    public WeatherController(IWeatherAdvisorService weatherAdvisorService)
    {
        _weatherAdvisorService = weatherAdvisorService;
    }

    // Each call spends Anthropic API tokens, so it is limited to authenticated users.
    [Authorize]
    [HttpGet("clothing")]
    public async Task<ActionResult<WeatherClothingDTO>> GetClothingAdvice(
        [FromQuery] string city,
        [FromQuery] decimal? lat,
        [FromQuery] decimal? lon,
        CancellationToken cancellationToken)
    {
        return await _weatherAdvisorService.GetClothingAdvice(city, lat, lon, cancellationToken);
    }
}
