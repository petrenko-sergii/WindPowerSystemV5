using System.ComponentModel.DataAnnotations;

namespace WindPowerSystemV5.Server.Data.Models;

public class TurbineType
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(32)]
    public string Manufacturer { get; set; } = string.Empty;

    [Required]
    [MaxLength(32)]
    public string Model { get; set; } = string.Empty;

    [Required]
    public float Capacity { get; set; }

    [Required]
    public string FileName { get; set; } = string.Empty;

    public ICollection<Turbine>? Turbines { get; set; }
}
