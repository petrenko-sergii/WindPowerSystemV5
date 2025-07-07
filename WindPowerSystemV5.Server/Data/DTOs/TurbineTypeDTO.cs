namespace WindPowerSystemV5.Server.Data.DTOs;

public class TurbineTypeDTO
{
    public int Id { get; set; }

    public string Manufacturer { get; set; } = string.Empty;

    public string Model { get; set; } = string.Empty;

    public float Capacity { get; set; }

    public string FileName { get; set; } = string.Empty;

    public int TurbineQty { get; set; }
}