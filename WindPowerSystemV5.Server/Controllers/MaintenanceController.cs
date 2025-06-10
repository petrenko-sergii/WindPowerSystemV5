using Microsoft.AspNetCore.Mvc;
using WindPowerSystemV5.Server.Data.NoSqlModels;
using WindPowerSystemV5.Server.Services;

namespace WindPowerSystemV5.Server.Controllers;

[Route("api/maintenance-record")]
[ApiController]
public class MaintenanceController : ControllerBase
{
    private readonly CosmosDbService _cosmosDbService;

    public MaintenanceController(CosmosDbService cosmosDbService)
    {
        _cosmosDbService = cosmosDbService;
    }

    [HttpGet("{id}")]
    public async Task<MaintenanceRecord?> GetRecord(string id)
    {
        return await _cosmosDbService.GetMaintenanceRecordAsync(id);
    }

    [HttpGet("turbine/{turbineId}")]
    public async Task<List<MaintenanceRecord>> GetRecordsByTurbineId(int turbineId)
    {
        return await _cosmosDbService.GetMaintenanceRecordsByTurbineIdAsync(turbineId);
    }
}
