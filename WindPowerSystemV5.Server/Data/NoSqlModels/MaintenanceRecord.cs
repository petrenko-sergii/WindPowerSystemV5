using Newtonsoft.Json;

namespace WindPowerSystemV5.Server.Data.NoSqlModels;

public class MaintenanceRecord
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public int TurbineId { get; set; }

    public DateTime ServiceDate { get; set; }

    public string Type { get; set; } = null!;

    public string Technician { get; set; } = null!;

    public List<string>? ActionsTaken { get; set; }

    public List<string>? Issues { get; set; }

    public string? Status { get; set; }

    public Dictionary<string, string>? Checklist { get; set; }

    public object? AdditionalData { get; set; }

    public int DowntimeMinutes { get; set; }

    public List<string> PartsReplaced { get; set; } = null!;

    public string EventTrigger { get; set; } = null!;

    public string Comments { get; set; } = null!;
}
