using HotChocolate.Authorization;
using Microsoft.EntityFrameworkCore;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.Utils.Exceptions;

namespace WindPowerSystemV5.Server.Data.GraphQL;

public class Mutation
{
    public TurbineMutation TurbineMutations => new();

    /// <summary>
    /// Add a new City
    /// </summary>
    [Serial]
    [Authorize(Roles = ["RegisteredUser"])]
    public async Task<City> AddCity(
        [Service] ApplicationDbContext context, CityDTO cityDTO)
    {
        var city = new City()
        {
            Name = cityDTO.Name,
            Lat = cityDTO.Lat,
            Lon = cityDTO.Lon,
            CountryId = cityDTO.CountryId
        };

        context.Cities.Add(city);
        await context.SaveChangesAsync();
        return city;
    }

    /// <summary>
    /// Update an existing City
    /// </summary>
    [Serial]
    [Authorize(Roles = ["RegisteredUser"])]
    public async Task<City> UpdateCity(
        [Service] ApplicationDbContext context, CityDTO cityDTO)
    {
        var city = await context.Cities
            .Where(c => c.Id == cityDTO.Id)
            .FirstOrDefaultAsync();
        
        if (city == null)
        {
            // todo: handle errors
            throw new NotSupportedException();
        }

        city.Name = cityDTO.Name;
        city.Lat = cityDTO.Lat;
        city.Lon = cityDTO.Lon;
        city.CountryId = cityDTO.CountryId;
        context.Cities.Update(city);
        await context.SaveChangesAsync();
        return city;
    }

    /// <summary>
    /// Delete a City
    /// </summary>
    [Serial]
    [Authorize(Roles = ["Administrator"])]
    public async Task DeleteCity(
        [Service] ApplicationDbContext context, int id)
    {
        var city = await context.Cities
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();

        if (city != null)
        {
            context.Cities.Remove(city);
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Add a new Country
    /// </summary>
    [Serial]
    [Authorize(Roles = ["RegisteredUser"])]
    public async Task<Country> AddCountry(
        [Service] ApplicationDbContext context, CountryDTO countryDTO)
    {
        var country = new Country()
        {
            Name = countryDTO.Name,
            ISO2 = countryDTO.ISO2,
            ISO3 = countryDTO.ISO3
        };
        context.Countries.Add(country);
        await context.SaveChangesAsync();
        return country;
    }

    /// <summary>
    /// Update an existing Country
    /// </summary>
    [Serial]
    [Authorize(Roles = ["RegisteredUser"])]
    public async Task<Country> UpdateCountry(
        [Service] ApplicationDbContext context, CountryDTO countryDTO)
    {
        var country = await context.Countries
            .Where(c => c.Id == countryDTO.Id)
            .FirstOrDefaultAsync();

        if (country == null) 
        { 
            // todo: handle errors
            throw new NotSupportedException();
        }

        country.Name = countryDTO.Name;
        country.ISO2 = countryDTO.ISO2;
        country.ISO3 = countryDTO.ISO3;
        context.Countries.Update(country);
        await context.SaveChangesAsync();
        return country;
    }

    /// <summary>
    /// Delete a Country
    /// </summary>
    [Serial]
    [Authorize(Roles = ["Administrator"])]
    public async Task DeleteCountry(
        [Service] ApplicationDbContext context, int id)
    {
        var country = await context.Countries
            .Where(c => c.Id == id)
            .FirstOrDefaultAsync();

        if (country != null)
        {
            context.Countries.Remove(country);
            await context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Update an existing TurbineType
    /// </summary>
    [Serial]
    [Authorize(Roles = ["RegisteredUser"])]
    public async Task<TurbineType> UpdateTurbineType(
        [Service] ApplicationDbContext context, TurbineType turbineType)
    {
        var turbineTypeToUpdate = await context.TurbineTypes
            .Where(t => t.Id == turbineType.Id)
            .FirstOrDefaultAsync();

        if (turbineTypeToUpdate == null)
        {
            throw new NotFoundException($"TurbineType with ID {turbineType.Id} is not found.");
        }

        turbineTypeToUpdate.Manufacturer = turbineType.Manufacturer;
        turbineTypeToUpdate.Model = turbineType.Model;
        turbineTypeToUpdate.Capacity = turbineType.Capacity;

        context.TurbineTypes.Update(turbineTypeToUpdate);
        await context.SaveChangesAsync();
        return turbineTypeToUpdate;
    }

    /// <summary>
    ﻿/// Delete a TurbineType
    /// </summary>
    [Serial]
    [Authorize(Roles = ["Administrator"])]
    public async Task<string> DeleteTurbineType(
        [Service] ApplicationDbContext context, int id)
    {
        var turbineType = await context.TurbineTypes
            .Where(t => t.Id == id)
            .FirstOrDefaultAsync();

        if (turbineType == null)
        {
            throw new NotFoundException($"TurbineType with ID {id} is not found.");
        }

        context.TurbineTypes.Remove(turbineType);
        await context.SaveChangesAsync();

        return $"TurbineType {turbineType.Model} with ID {turbineType.Id} was deleted ";
    }
}