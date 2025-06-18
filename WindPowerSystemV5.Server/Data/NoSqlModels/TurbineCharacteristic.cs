using Newtonsoft.Json;

namespace WindPowerSystemV5.Server.Data.NoSqlModels;

public class TurbineCharacteristic
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [JsonProperty(PropertyName = "turbineId")]
    public int TurbineId { get; set; }

    public float RotorDiameter { get; set; }

    public float HubHeight { get; set; }

    public double RotorSpeedRpm { get; set; }

    public double WindSpeedMps { get; set; }

    public double PowerOutputKw { get; set; }

    public DateTime Timestamp { get; set; }
}
