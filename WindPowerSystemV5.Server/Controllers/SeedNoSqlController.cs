using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using WindPowerSystemV5.Server.Data.NoSqlModels;

namespace WindPowerSystemV5.Server.Controllers;

[Route("api/seed-no-sql")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class SeedNoSqlController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public SeedNoSqlController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpPost("maintenance-records")]
    public async Task<IActionResult> SeedMaintenanceRecords()
    {
        var cosmosDbEndpoint = _configuration["CosmosDb:Endpoint"];
        var cosmosDbKey = _configuration["CosmosDb:PrimaryKey"];
        var databaseName = _configuration["CosmosDb:Database"];
        var containerName = "MaintenanceRecords";

        var client = new CosmosClient(cosmosDbEndpoint, cosmosDbKey);
        var container = client.GetContainer(databaseName, containerName);

        var records = new List<MaintenanceRecord>
        {
            new MaintenanceRecord
            {
                TurbineId = 1,
                ServiceDate = DateTime.Parse("2025-05-01"),
                Type = "Preventive",
                Technician = "John Smith",
                ActionsTaken = ["Checked oil level", "Cleaned rotor hub"],
                Status = "Completed"
            },
            new MaintenanceRecord
            {
                TurbineId = 2,
                ServiceDate = DateTime.Parse("2025-05-15"),
                Type = "Corrective",
                DowntimeMinutes = 180,
                Issues = ["Main shaft vibration", "Gearbox overheating"],
                ActionsTaken = ["Replaced vibration sensor", "Refilled gearbox oil"],
                PartsReplaced = ["VibrationSensor-X2"],
                Technician = "Jacob Hansen",
                Status = "Completed"
            },
            new MaintenanceRecord
            {
                TurbineId = 3,
                ServiceDate = DateTime.Parse("2025-06-02"),
                Type = "Emergency",
                EventTrigger = "Remote alarm - emergency stop",
                Comments = "Emergency brake engaged due to rapid RPM increase.",
                ActionsTaken = ["Manually reset brake system", "Inspected rotor shaft"],
                Status = "Resolved"
            }
        };

        foreach (var record in records)
        {
            await container.CreateItemAsync(record);
        }

        return Ok(new { message = "Seeded MaintenanceRecords to Cosmos DB." });
    }
}
