using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.Data;
using WindPowerSystemV5.Server.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using WindPowerSystemV5.Server.Data.Repositories.Interfaces;

namespace WindPowerSystemV5.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CitiesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ICityRepository _cityRepository;

    public CitiesController(ApplicationDbContext context, ICityRepository cityRepository)
    {
        _context = context;
        _cityRepository = cityRepository;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<CityDTO>>> GetCities(
           int pageIndex = 0,
           int pageSize = 10,
           string? sortColumn = null,
           string? sortOrder = null,
           string? filterColumn = null,
           string? filterQuery = null)
    {
        return await ApiResult<CityDTO>.CreateAsync(
                _context.Cities.AsNoTracking()
                .Select(c => new CityDTO()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Lat = c.Lat,
                    Lon = c.Lon,
                    CountryId = c.Country!.Id,
                    CountryName = c.Country!.Name
                }),
                pageIndex,
                pageSize,
                sortColumn,
                sortOrder,
                filterColumn,
                filterQuery);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<City>> GetCity(int id)
    {
        var city = await _context.Cities.FindAsync(id);

        if (city == null)
        {
            return NotFound();
        }

        return city;
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "RegisteredUser")]
    public async Task<IActionResult> PutCity(int id, City city)
    {
        if (id != city.Id)
        {
            return BadRequest();
        }

        _context.Entry(city).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CityExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpPost]
    [Authorize(Roles = "RegisteredUser")]
    public async Task<ActionResult<City>> PostCity(City city)
    {
        await _cityRepository.Create(city);
   
        return CreatedAtAction("GetCity", new { id = city.Id }, city);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteCity(int id)
    {
        var city = await _context.Cities.FindAsync(id);
        if (city == null)
        {
            return NotFound();
        }

        _context.Cities.Remove(city);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost]
    [Route("IsDupeCity")]
    public bool IsDupeCity(City city)
    {
        return _context.Cities.AsNoTracking().Any(
            c => c.Name == city.Name
            && c.Lat == city.Lat
            && c.Lon == city.Lon
            && c.CountryId == city.CountryId
            && c.Id != city.Id
        );
    }

    private bool CityExists(int id)
    {
        return _context.Cities.AsNoTracking().Any(e => e.Id == id);
    }
}