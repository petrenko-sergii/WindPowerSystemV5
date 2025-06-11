using Microsoft.AspNetCore.Mvc;
using WindPowerSystemV5.Server.Data.NoSqlModels;
using WindPowerSystemV5.Server.Services.Interfaces;

namespace WindPowerSystemV5.Server.Controllers;

[Route("api/maintenance-record")]
[ApiController]
public class MaintenanceRecordController : ControllerBase
{
    private readonly IMaintenanceRecordService _maintenanceRecordService;

    public MaintenanceRecordController(IMaintenanceRecordService maintenanceRecordService)
    {
        _maintenanceRecordService = maintenanceRecordService;
    }

    [HttpGet("{id}")]
    public async Task<MaintenanceRecord?> Get(string id)
    {
        return await _maintenanceRecordService.Get(id);
    }

    [HttpGet("turbine/{turbineId}")]
    public async Task<List<MaintenanceRecord>> GetByTurbineId(int turbineId)
    {
        return await _maintenanceRecordService.GetByTurbineId(turbineId);
    }
}
