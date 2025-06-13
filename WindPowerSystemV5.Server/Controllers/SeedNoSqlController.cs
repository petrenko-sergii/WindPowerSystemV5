using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.Cosmos;
using Microsoft.AspNetCore.Mvc;
using WindPowerSystemV5.Server.Data.NoSqlModels;
using WindPowerSystemV5.Server.Services.Interfaces;

namespace WindPowerSystemV5.Server.Controllers;

[Route("api/seed-no-sql")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class SeedNoSqlController : ControllerBase
{
    private readonly ICosmosDbContext _cosmosDbContext;

    public SeedNoSqlController(ICosmosDbContext cosmosDbContext)
    {
        _cosmosDbContext = cosmosDbContext;
    }

    [HttpPost("maintenance-records")]
    public async Task<IActionResult> SeedMaintenanceRecords()
    {
        var container = _cosmosDbContext.MaintenanceRecordsContainer;

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

    [HttpPost("turbine-config-snapshots")]
    public async Task<IActionResult> SeedTurbineConfigSnapshots()
    {
        var database = _cosmosDbContext.TurbineConfigSnapshotsContainer.Database;
        var containerId = ContainerNames.TurbineConfigSnapshots;
        var partitionKeyPath = "/turbineId";

        var containerResponse = await database.CreateContainerIfNotExistsAsync(
            new ContainerProperties
            {
                Id = containerId,
                PartitionKeyPath = partitionKeyPath
            });

        var container = containerResponse.Container;

        var snapshots = new List<TurbineConfigSnapshot>
        {
            new() {
                TurbineId = 1,
                Timestamp = DateTime.Parse("2025-06-01T12:00:00Z"),
                FirmwareVersion = "v3.1.2",
                OperatingMode = "Auto",
                RatedPowerKW = 2500,
                BladePitchAngle = 12.5,
                YawAngle = 90,
                SensorCalibration = new Dictionary<string, double>
                {
                    { "temperature", 0.98 },
                    { "windSpeed", 1.02 }
                }
            },
            new() {
                TurbineId = 2,
                Timestamp = DateTime.Parse("2025-06-03T14:20:00Z"),
                FirmwareVersion = "v3.2.0-beta",
                OperatingMode = "Manual",
                BladeConfig = new BladeConfig
                {
                    MaxPitch = 18,
                    MinPitch = 5,
                    DynamicAdjustmentEnabled = true
                },
                CustomSettings = new Dictionary<string, object>
                {
                    { "debugLogs", true },
                    { "developerMode", true }
                }
            },
            new() {
                TurbineId = 3,
                Timestamp = DateTime.Parse("2025-05-30T09:45:00Z"),
                FirmwareVersion = "v2.9.5",
                OperatingMode = "Auto",
                EnvironmentalLimits = new EnvironmentalLimits
                {
                    MaxWindSpeedMps = 25,
                    MinTemperatureC = -20,
                    MaxTemperatureC = 50
                },
                ShutdownOnLimitBreach = true
            }
        };

        foreach (var snapshot in snapshots)
        {
            await container.CreateItemAsync(snapshot, new PartitionKey(snapshot.TurbineId));
        }

        var wasCreated = containerResponse.StatusCode == System.Net.HttpStatusCode.Created;

        var message = wasCreated
            ? $"Container {containerId} was created. "
            : string.Empty;

        message = string.Concat(message, "Seeded TurbineConfigSnapshot items to Cosmos DB.");

        return Ok(new { message });
    }
}
