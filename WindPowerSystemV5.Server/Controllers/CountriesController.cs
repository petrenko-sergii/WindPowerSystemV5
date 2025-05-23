﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.Data;
using WindPowerSystemV5.Server.Data.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace WindPowerSystemV5.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CountriesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CountriesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResult<CountryDTO>>> GetCountries(
    int pageIndex = 0,
    int pageSize = 10,
    string? sortColumn = null,
    string? sortOrder = null,
    string? filterColumn = null,
    string? filterQuery = null)
    {
        return await ApiResult<CountryDTO>.CreateAsync(
                _context.Countries.AsNoTracking()
                    .Select(c => new CountryDTO()
                    {
                        Id = c.Id,
                        Name = c.Name,
                        ISO2 = c.ISO2,
                        ISO3 = c.ISO3,
                        CityQty = c.Cities!.Count
                    }),
                pageIndex,
                pageSize,
                sortColumn,
                sortOrder,
                filterColumn,
                filterQuery);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Country>> GetCountry(int id)
    {
        var country = await _context.Countries.FindAsync(id);

        if (country == null)
        {
            return NotFound();
        }

        return country;
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "RegisteredUser")]
    public async Task<IActionResult> PutCountry(int id, Country country)
    {
        if (id != country.Id)
        {
            return BadRequest();
        }

        _context.Entry(country).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CountryExists(id))
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
    public async Task<ActionResult<Country>> PostCountry(Country country)
    {
        _context.Countries.Add(country);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCountry", new { id = country.Id }, country);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteCountry(int id)
    {
        var country = await _context.Countries.FindAsync(id);
        if (country == null)
        {
            return NotFound();
        }

        _context.Countries.Remove(country);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost]
    [Route("IsDupeField")]
    public bool IsDupeField(
        int countryId,
        string fieldName,
        string fieldValue)
    {
        switch (fieldName)
        {
            case "name":
                return _context.Countries.Any(
                c => c.Name == fieldValue && c.Id != countryId);
            case "iso2":
                return _context.Countries.Any(
                    c => c.ISO2 == fieldValue && c.Id != countryId);
            case "iso3":
                return _context.Countries.Any(
                c => c.ISO3 == fieldValue && c.Id != countryId);
            default:
                return false;
        }
    }

    private bool CountryExists(int id)
    {
        return _context.Countries.Any(e => e.Id == id);
    }
}