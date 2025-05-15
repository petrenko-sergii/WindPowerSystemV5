namespace WindPowerSystemV5.Server.Data.DTOs;

public class TurbineDTO
{
    public int Id { get; set; }

    public string SerialNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int TurbineTypeId { get; set; }

    public string Manufacturer { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;
}
