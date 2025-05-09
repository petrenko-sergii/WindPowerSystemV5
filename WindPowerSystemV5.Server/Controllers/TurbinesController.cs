using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.Data;

namespace WindPowerSystemV5.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TurbinesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TurbinesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Turbine>>> Get()
    {
        return await _context.Turbines.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Turbine>> Get(int id)
    {
        var turbine = await _context.Turbines.FindAsync(id);

        if (turbine == null)
        {
            return NotFound();
        }

        return turbine;
    }
}
