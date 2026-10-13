using Microsoft.EntityFrameworkCore;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.Data.Repositories.Interfaces;
using WindPowerSystemV5.Server.ViewModels;

namespace WindPowerSystemV5.Server.Data.Repositories;

public class TurbineTypeRepository : ITurbineTypeRepository
{
    private readonly ApplicationDbContext _context;

    public TurbineTypeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TurbineType?> Get(int id)
    {
        return await _context.TurbineTypes.FindAsync(id);
    }

    public async Task<int> Create(TurbineType turbineType)
    {
        _context.TurbineTypes.Add(turbineType);
        await _context.SaveChangesAsync();

        return turbineType.Id;
    }

    public async Task<TurbineType?> GetByFileName(string fileName)
    {
        return  await _context.TurbineTypes
            .SingleOrDefaultAsync(t => t.FileName == fileName);
    }

    public async Task<List<TurbineTypeDTO>> Search(TurbineTypeSearchCriteria criteria)
    {
        var query = _context.TurbineTypes.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(criteria.Manufacturer))
        {
            query = query.Where(t => t.Manufacturer.Contains(criteria.Manufacturer));
        }

        if (!string.IsNullOrWhiteSpace(criteria.Model))
        {
            query = query.Where(t => t.Model.Contains(criteria.Model));
        }

        if (criteria.MinCapacity.HasValue)
        {
            query = query.Where(t => t.Capacity >= criteria.MinCapacity.Value);
        }

        if (criteria.MaxCapacity.HasValue)
        {
            query = query.Where(t => t.Capacity <= criteria.MaxCapacity.Value);
        }

        var projected = query.Select(t => new TurbineTypeDTO
        {
            Id = t.Id,
            Manufacturer = t.Manufacturer,
            Model = t.Model,
            Capacity = t.Capacity,
            FileName = t.FileName,
            TurbineQty = t.Turbines != null ? t.Turbines.Count : 0
        });

        if (criteria.MinTurbineQty.HasValue)
        {
            projected = projected.Where(t => t.TurbineQty >= criteria.MinTurbineQty.Value);
        }

        var descending = criteria.SortDescending;
        projected = criteria.SortBy switch
        {
            TurbineTypeSearchCriteria.SortByCapacity => descending
                ? projected.OrderByDescending(t => t.Capacity) : projected.OrderBy(t => t.Capacity),
            TurbineTypeSearchCriteria.SortByManufacturer => descending
                ? projected.OrderByDescending(t => t.Manufacturer) : projected.OrderBy(t => t.Manufacturer),
            TurbineTypeSearchCriteria.SortByModel => descending
                ? projected.OrderByDescending(t => t.Model) : projected.OrderBy(t => t.Model),
            TurbineTypeSearchCriteria.SortByTurbineQty => descending
                ? projected.OrderByDescending(t => t.TurbineQty) : projected.OrderBy(t => t.TurbineQty),
            _ => projected.OrderBy(t => t.Id)
        };

        var limit = Math.Clamp(criteria.Limit, 1, TurbineTypeSearchCriteria.MaxLimit);

        return await projected.Take(limit).ToListAsync();
    }
}
