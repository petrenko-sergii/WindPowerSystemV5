using Microsoft.EntityFrameworkCore;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.Data.Repositories.Interfaces;

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
}
