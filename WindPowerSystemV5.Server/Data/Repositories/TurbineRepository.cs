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
}
