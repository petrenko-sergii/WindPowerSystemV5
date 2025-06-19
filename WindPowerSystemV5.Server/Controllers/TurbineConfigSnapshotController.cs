using Microsoft.AspNetCore.Mvc;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Services.Interfaces;
using WindPowerSystemV5.Server.Utils.Exceptions;

namespace WindPowerSystemV5.Server.Controllers;

[Route("api/turbine-config-snapshot")]
[ApiController]
public class TurbineConfigSnapshotController : ControllerBase
{
    private readonly ITurbineConfigSnapshotService _turbineConfigSnapshotService;

    public TurbineConfigSnapshotController(ITurbineConfigSnapshotService turbineConfigSnapshotService)
    {
        _turbineConfigSnapshotService = turbineConfigSnapshotService;
    }

    [HttpGet("{id}")]
    public async Task<TurbineConfigSnapshotDTO?> Get(string id)
    {
        return await _turbineConfigSnapshotService.Get(id);
    }

    [HttpGet("operating-mode/{operatingMode}")]
    public async Task<List<TurbineConfigSnapshotDTO>> GetByOperatingMode(string operatingMode)
    {
        return await _turbineConfigSnapshotService.GetByOperatingMode(operatingMode);
    }

    [HttpGet("turbine/{turbineId}")]
    public async Task<List<TurbineConfigSnapshotDTO>> GetByTurbineId(int turbineId)
    {
        return await _turbineConfigSnapshotService.GetByTurbineId(turbineId);
    }

    [HttpPut("{id}")]
    public async Task Update(string id, TurbineConfigSnapshotDTO recordToUpdate)
    {
        if (recordToUpdate == null || id != recordToUpdate.Id)
        {
            throw new BadRequestException("Invalid turbine config snapshot data or mismatched id.");
        }

        await _turbineConfigSnapshotService.Update(id, recordToUpdate);
    }
}
