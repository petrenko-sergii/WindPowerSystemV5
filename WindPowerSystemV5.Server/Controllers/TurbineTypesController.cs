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
                FileName = t.FileName,
                TurbineQty = t.Turbines!.Count
            })
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<TurbineTypeDTO?>> Get(int id)
    {
        return await _turbineTypeService.Get(id);
    }

    [HttpGet("download-info-file")]
    public async Task<IActionResult> DownloadInfoFile([FromQuery] string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
        {
            throw new BadRequestException("File name is required.");
        }

        var fileResult = await _blobStorageService.DownloadFileAsync(fileName);

        if (fileResult is null)
        {
            throw new NotFoundException($"Info-file with the next name \"{fileName}\" is not found.");
        }

        fileResult.FileDownloadName = await _turbineTypeService.NameInfoFile(fileName); ;

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
