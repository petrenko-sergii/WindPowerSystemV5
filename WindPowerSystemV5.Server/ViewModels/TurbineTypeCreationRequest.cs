using System.ComponentModel.DataAnnotations;

namespace WindPowerSystemV5.Server.ViewModels;

public class TurbineTypeCreationRequest
{
    [Required]
    [MaxLength(32)]
    public string Manufacturer { get; set; } = string.Empty;

    [Required]
    [MaxLength(32)]
    public string Model { get; set; } = string.Empty;

    [Required]
    public float Capacity { get; set; }
}
