
namespace WindPowerSystemV5.Server.Data.DTOs;

public class MaintenanceRecordDTO
{
    public string Id { get; set; }

    public int TurbineId { get; set; }

    public DateTime ServiceDate { get; set; }

    public string Type { get; set; }

    public string Technician { get; set; }

    public List<string>? ActionsTaken { get; set; }

    public List<string>? Issues { get; set; }

    public string? Status { get; set; }

    public Dictionary<string, string>? Checklist { get; set; }

    public object? AdditionalData { get; set; }

    public int DowntimeMinutes { get; set; }

    public List<string> PartsReplaced { get; set; }

    public string EventTrigger { get; set; }

    public string Comments { get; set; }
}
