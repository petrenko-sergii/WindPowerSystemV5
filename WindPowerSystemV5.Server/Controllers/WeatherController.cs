using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Services.Interfaces;

namespace WindPowerSystemV5.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class WeatherController : ControllerBase
{
    private readonly IWeatherAdvisorAgent _weatherAdvisorAgent;

    public WeatherController(IWeatherAdvisorAgent weatherAdvisorAgent)
    {
        _weatherAdvisorAgent = weatherAdvisorAgent;
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
        return await _weatherAdvisorAgent.GetClothingAdvice(city, lat, lon, cancellationToken);
    }
}
