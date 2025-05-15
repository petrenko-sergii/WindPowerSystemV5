using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WindPowerSystemV5.Server.Data;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.Data.DTOs;

namespace WindPowerSystemV5.Server.Controllers;

[Route("api/turbine-types")]
[ApiController]
public class TurbineTypesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TurbineTypesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TurbineTypeDTO>>> Get()
    {
        return await _context.TurbineTypes
            .Select(t => new TurbineTypeDTO
            {
                Id = t.Id,
                Manufacturer = t.Manufacturer,
                Model = t.Model,
                Capacity = t.Capacity,
                TurbineQty = t.Turbines!.Count
            })
            .ToListAsync();
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

    [HttpPost]
    public async Task<ActionResult<TurbineType>> Create(TurbineType turbineType)
    {
        _context.TurbineTypes.Add(turbineType);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = turbineType.Id }, turbineType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, TurbineType turbineType)
    {
        if (id != turbineType.Id)
        {
            return BadRequest();
        }

        _context.Entry(turbineType).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!TurbineTypeExists(id))
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

    private bool TurbineTypeExists(int id)
    {
        return _context.TurbineTypes.AsNoTracking().Any(e => e.Id == id);
    }
}
