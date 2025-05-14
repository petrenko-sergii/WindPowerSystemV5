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

    [HttpPost]
    public async Task<ActionResult<Turbine>> Create(Turbine turbine)
    {
        _context.Turbines.Add(turbine);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = turbine.Id }, turbine);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Turbine turbine)
    {
        if (id != turbine.Id)
        {
            return BadRequest();
        }

        _context.Entry(turbine).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TurbineExists(id))
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

    private bool TurbineExists(int id)
    {
        return _context.Turbines.AsNoTracking().Any(e => e.Id == id);
    }
}
