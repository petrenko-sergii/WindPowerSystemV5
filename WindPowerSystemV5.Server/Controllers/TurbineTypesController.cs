using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.Data;

namespace WindPowerSystemV5.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TurbineTypesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TurbineTypesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TurbineType>>> Get()
    {
        return await _context.TurbineTypes.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TurbineType>> Get(int id)
    {
        var turbineType = await _context.TurbineTypes.FindAsync(id);

        if (turbineType == null)
        {
            return NotFound();
        }

        return turbineType;
    }
}
