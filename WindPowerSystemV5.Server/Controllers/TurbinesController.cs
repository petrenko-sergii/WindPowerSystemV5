using Microsoft.AspNetCore.Mvc;
using WindPowerSystemV5.Server.Data.DTOs;
using Microsoft.AspNetCore.Authorization;
using WindPowerSystemV5.Server.Services.Interfaces;

namespace WindPowerSystemV5.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TurbinesController : ControllerBase
{
    private readonly ITurbineService _turbineService;

    public ILogger<TurbinesController> Logger { get; set; }

    public TurbinesController(
        ITurbineService turbineService,
        ILogger<TurbinesController> logger)
    {
        _turbineService = turbineService;
        Logger = logger;
        Logger.LogInformation("TurbinesController initialized.");
    }

    [HttpGet]
    public async Task<List<TurbineDTO>> Get()
    {
        return await _turbineService.Get();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TurbineDTO))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<TurbineDTO> Get([FromRoute] int id)
    {
        return await _turbineService.Get(id);
    }

    [HttpPost]
    [Authorize(Roles = "RegisteredUser")]
    public async Task<int> Create([FromBody] TurbineDTO turbineDto)
    {
        return await _turbineService.Create(turbineDto);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "RegisteredUser")]
    public async Task Update([FromRoute] int id, [FromBody] TurbineDTO turbineDto)
    {
        await _turbineService.Update(turbineDto);
    }

    [HttpGet("statuses")]
    public List<string> GetStatuses()
    {
        return [.. Enum.GetNames(typeof(Data.Enums.TurbineStatus))];
    }
}
