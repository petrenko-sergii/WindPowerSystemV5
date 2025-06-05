using Microsoft.AspNetCore.Mvc;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Services.Interfaces;

namespace WindPowerSystemV5.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AddressesController : ControllerBase
{
    private readonly ICityService _cityService;

    public ILogger<AddressesController> Logger { get; set; }

    public AddressesController(
        ICityService cityService,
        ILogger<AddressesController> logger)
    {
        _cityService = cityService;
        Logger = logger;
        Logger.LogInformation("AddressesController initialized.");
    }

    [HttpGet("cities")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CityDTO>))]
    public async Task<List<CityDTO>> GetCities()
    {
        return await _cityService.Get();
    }

    [HttpGet("cities-with-country")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CityDTO>))]
    public async Task<List<CityDTO>> GetCitiesWithCountry()
    {
        return await _cityService.GetWithCountry();
    }

    [HttpGet("cached-city/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CityDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<CityDTO> GetCachedCity(int id)
    {
        return await _cityService.GetCached(id);
    }
}
