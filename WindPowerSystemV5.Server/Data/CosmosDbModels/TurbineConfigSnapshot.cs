using Newtonsoft.Json;

namespace WindPowerSystemV5.Server.Data.CosmosDbModels;

public class TurbineConfigSnapshot
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty(PropertyName = "turbineId")]
    public int TurbineId { get; set; }

    public DateTime Timestamp { get; set; }

    public string FirmwareVersion { get; set; } = default!;

    public string OperatingMode { get; set; } = default!;

    public double? RatedPowerKW { get; set; }

    public double? BladePitchAngle { get; set; }

    public double? YawAngle { get; set; }

    public Dictionary<string, double>? SensorCalibration { get; set; }

    public BladeConfig? BladeConfig { get; set; }

    public Dictionary<string, object>? CustomSettings { get; set; }

    public EnvironmentalLimits? EnvironmentalLimits { get; set; }

    public bool? ShutdownOnLimitBreach { get; set; }

    public List<SensorEntry>? SensorMap { get; set; }
    
    public string? HealthStatus { get; set; }
    
    public bool? AlertsEnabled { get; set; }

    public string? Notes { get; set; }

    public bool? Archived { get; set; }

    public OperatorInfo? OperatorInfo { get; set; }
}

public class BladeConfig
{
    public double MaxPitch { get; set; }

    public double MinPitch { get; set; }

    public bool DynamicAdjustmentEnabled { get; set; }
}

public class EnvironmentalLimits
{
    public double MaxWindSpeedMps { get; set; }

    public double MinTemperatureC { get; set; }

    public double MaxTemperatureC { get; set; }
}

public class SensorEntry
{
    public string Type { get; set; } = default!;

    public string Location { get; set; } = default!;

    public double Calibration { get; set; }
}

public class OperatorInfo
{
    public string Name { get; set; } = default!;

    public string Shift { get; set; } = default!;
}
