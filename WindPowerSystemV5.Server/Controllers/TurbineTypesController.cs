using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WindPowerSystemV5.Server.Data;
using WindPowerSystemV5.Server.Data.Models;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.ViewModels;
using WindPowerSystemV5.Server.Services.Interfaces;
using WindPowerSystemV5.Server.Utils.Exceptions;
namespace WindPowerSystemV5.Server.Controllers;

[Route("api/turbine-types")]
[ApiController]
public class TurbineTypesController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly ITurbineTypeService _turbineTypeService;
    private readonly IBlobStorageService _blobStorageService;

    public ILogger<TurbineTypesController> Logger { get; set; }

    public TurbineTypesController(
        ApplicationDbContext context, 
        ILogger<TurbineTypesController> logger,
        ITurbineTypeService turbineTypeService,
        IBlobStorageService blobStorageService)
    {
        _context = context;
        _turbineTypeService = turbineTypeService;
        _blobStorageService = blobStorageService;
        Logger = logger;
        Logger.LogInformation("TurbineTypesController initialized.");
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
                InfoFileUri = t.InfoFileUri,
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

    [HttpGet("download-info-file")]
    public async Task<IActionResult> DownloadInfoFile([FromQuery] string uri)
    {
        if (string.IsNullOrWhiteSpace(uri))
        {
            throw new BadRequestException("File URI is required.");
        }

        var fileResult = await _blobStorageService.DownloadFileAsync(uri);
        if (fileResult is null)
        {
            throw new NotFoundException($"Info-file with the next uri \"{uri}\" is not found.");
        }

        return fileResult;
    }

    [HttpPost]
    [Authorize(Roles = "RegisteredUser")]
    public async Task<int> Create(
        [FromForm] TurbineTypeCreationRequest turbineTypeCreationRequest,
        IFormFile infoFile)
    {
        return await _turbineTypeService.Create(turbineTypeCreationRequest, infoFile);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "RegisteredUser")]
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
