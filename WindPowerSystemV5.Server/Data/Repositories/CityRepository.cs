using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Caching.Memory;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.Data.Repositories.Interfaces;

namespace WindPowerSystemV5.Server.Data.Repositories;

public class CityRepository : ICityRepository
{
    private readonly ApplicationDbContext _context;
    private readonly IMemoryCache _memoryCache;

    private readonly MemoryCacheEntryOptions _cacheEntryOptions = new()
    {
        SlidingExpiration = TimeSpan.FromMinutes(30)
    };

    public CityRepository(ApplicationDbContext context, IMemoryCache memoryCache)
    {
        _context = context;
        _memoryCache = memoryCache;
    }

    public async Task<List<City>> Get()
    {
        return await _context.Cities
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<City>> GetWithCountry()
    {
        return await _context.Cities
            .Include(c => c.Country)
            .AsNoTracking()
            .AsSplitQuery()
            .ToListAsync();
    }

    public async Task<City?> Create(City city)
    {
        // Add to database using EF Core.
        EntityEntry<City> added = await _context.Cities.AddAsync(city);
        int affected = await _context.SaveChangesAsync();
        
        if (affected == 1)
        {
            // If saved to database then store in cache.
            _memoryCache.Set(city.Id, city, _cacheEntryOptions);
            return city;
        }

        return null;
    }

    public async Task<City?> GetCached(int id)
    {
        // Try to get from the cache first.
        if (_memoryCache.TryGetValue(id, out City? fromCache))
        {
            return await Task.FromResult(fromCache);
        }

        // If not in the cache, then try to get it from the database.
        City? fromDb = _context.Cities.FirstOrDefault(c => c.Id ==  id);

        // If not -in database then return null result.
        if (fromDb is null) 
        { 
            return null; 
        }

        // If in the database, then store in the cache and return city.
        _memoryCache.Set(fromDb.Id, fromDb, _cacheEntryOptions);
       
        return await Task.FromResult(fromDb)!;
    }
}
