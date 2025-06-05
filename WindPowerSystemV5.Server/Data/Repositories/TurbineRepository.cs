using Microsoft.EntityFrameworkCore;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.Data.Repositories.Interfaces;

namespace WindPowerSystemV5.Server.Data.Repositories;

public class TurbineRepository : ITurbineRepository
{
    private readonly ApplicationDbContext _context;

    public TurbineRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Turbine>> Get()
    {
        return await _context.Turbines
            .Include(t => t.TurbineType)
            .ToListAsync();
    }

    public async Task<Turbine?> Get(int id)
    {
        return await _context.Turbines.FindAsync(id);
    }

    public async Task<int> Create(Turbine turbine)
    {
        _context.Turbines.Add(turbine);
        await _context.SaveChangesAsync();

        return turbine.Id;
    }

    public async Task Update(Turbine turbine)
    {
        var local = _context.Set<Turbine>().Local.FirstOrDefault(entry => entry.Id.Equals(turbine.Id));

        if (local != null)
        {
            _context.Entry(local).State = EntityState.Detached;
        }

        _context.Entry(turbine).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}
