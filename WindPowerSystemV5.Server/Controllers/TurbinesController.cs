using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WindPowerSystemV5.Server.Data;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.Data.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace WindPowerSystemV5.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TurbinesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    
    public ILogger<TurbinesController> Logger { get; set; }

    public TurbinesController(ApplicationDbContext context, ILogger<TurbinesController> logger)
    {
        _context = context;
        Logger = logger;
        Logger.LogInformation("TurbinesController initialized.");
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TurbineDTO>>> Get()
    {
        return await _context.Turbines
            .Select(t => new TurbineDTO
            {
                Id = t.Id,
                SerialNumber = t.SerialNumber,
                Status = t.Status.ToString(),
                TurbineTypeId = t.TurbineTypeId,
                Manufacturer = t.TurbineType!.Manufacturer,
                Model = t.TurbineType!.Model
            })
            .ToListAsync();
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
    [Authorize(Roles = "RegisteredUser")]
    public async Task<ActionResult<Turbine>> Create(Turbine turbine)
    {
        _context.Turbines.Add(turbine);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = turbine.Id }, turbine);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "RegisteredUser")]
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

    [HttpGet("statuses")]
    public List<string> GetStatuses()
    {
        return Enum.GetNames(typeof(Data.Enums.TurbineStatus)).ToList();
    }

    private bool TurbineExists(int id)
    {
        return _context.Turbines.AsNoTracking().Any(e => e.Id == id);
    }
}
