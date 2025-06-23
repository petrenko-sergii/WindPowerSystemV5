using Microsoft.EntityFrameworkCore;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Data.Models;

namespace WindPowerSystemV5.Server.Data.GraphQL;

public class Query
{
    /// <summary>
    /// Gets all Cities.
    /// </summary>
    [Serial]
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public IQueryable<City> GetCities(
        [Service] ApplicationDbContext context)
        => context.Cities;

    /// <summary>
    /// Gets all Countries.
    /// </summary>
    [Serial]
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Country> GetCountries(
        [Service] ApplicationDbContext context)
        => context.Countries;

    /// <summary>
    /// Gets all Cities (with ApiResult and DTO support).
    /// </summary>
    [Serial]
    public async Task<ApiResult<CityDTO>> GetCitiesApiResult(
        [Service] ApplicationDbContext context,
        int pageIndex = 0,
        int pageSize = 10,
        string? sortColumn = null,
        string? sortOrder = null,
        string? filterColumn = null,
        string? filterQuery = null)
    {
        return await ApiResult<CityDTO>.CreateAsync(
                context.Cities.AsNoTracking()
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

    /// <summary>
    /// Gets all Turbine Types
    /// </summary>
    [Serial]
    public IQueryable<TurbineTypeDTO> GetTurbineTypes(
        [Service] ApplicationDbContext context)
        => context.TurbineTypes.Select(t => new TurbineTypeDTO
        {
            Id = t.Id,
            Manufacturer = t.Manufacturer,
            Model = t.Model,
            Capacity = t.Capacity,
            TurbineQty = t.Turbines != null ? t.Turbines.Count : 0
        });

    /// <summary>
    /// Gets a TurbineType by id.
    /// </summary>
    [Serial]
    public async Task<TurbineType?> GetTurbineType(
        [Service] ApplicationDbContext context,
        int id)
    {
        return await context.TurbineTypes.FindAsync(id);
    }

    /// <summary>
    /// Gets all Turbines
    /// </summary>
    [Serial]
    public IQueryable<TurbineDTO> GetTurbines(
        [Service] ApplicationDbContext context)
        => context.Turbines
            .Include(t => t.TurbineType)
            .Select(t => new TurbineDTO {
                Id = t.Id,
                SerialNumber = t.SerialNumber,
                Status = t.Status.ToString(),
                TurbineTypeId = t.TurbineTypeId,
                Manufacturer = t.TurbineType!.Manufacturer,
                Model = t.TurbineType!.Model
            });
}