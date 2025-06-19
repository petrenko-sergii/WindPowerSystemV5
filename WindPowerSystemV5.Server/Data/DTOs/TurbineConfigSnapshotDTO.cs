namespace WindPowerSystemV5.Server.Data.DTOs;

public class TurbineConfigSnapshotDTO
{
    public string Id { get; set; } = null!;

    public int TurbineId { get; set; }

    public DateTime Timestamp { get; set; }

    public string FirmwareVersion { get; set; } = null!;

    public string OperatingMode { get; set; } = null!;

    public double? RatedPowerKW { get; set; }

    public double? BladePitchAngle { get; set; }

    public double? YawAngle { get; set; }

    public Dictionary<string, double>? SensorCalibration { get; set; }

    public BladeConfigDTO? BladeConfig { get; set; }

    public Dictionary<string, object>? CustomSettings { get; set; }

    public EnvironmentalLimitsDTO? EnvironmentalLimits { get; set; }

    public bool? ShutdownOnLimitBreach { get; set; }

    public List<SensorEntryDTO>? SensorMap { get; set; }
    
    public string? HealthStatus { get; set; }
    
    public bool? AlertsEnabled { get; set; }

    public string? Notes { get; set; }

    public bool? Archived { get; set; }

    public OperatorInfoDTO? OperatorInfo { get; set; }
}

public class BladeConfigDTO
{
    public double MaxPitch { get; set; }

    public double MinPitch { get; set; }

    public bool DynamicAdjustmentEnabled { get; set; }
}

public class EnvironmentalLimitsDTO
{
    public double MaxWindSpeedMps { get; set; }

    public double MinTemperatureC { get; set; }

    public double MaxTemperatureC { get; set; }
}

public class SensorEntryDTO
{
    public string Type { get; set; } = null!;

    public string Location { get; set; } = null!;

    public double Calibration { get; set; }
}

public class OperatorInfoDTO
{
    public string Name { get; set; } = null!;

    public string Shift { get; set; } = null!;
}
