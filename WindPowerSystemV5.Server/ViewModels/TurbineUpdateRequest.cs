using System.ComponentModel.DataAnnotations;

namespace WindPowerSystemV5.Server.ViewModels;

public class TurbineUpdateRequest
{
    [Required]
    public int Id { get; set; }

    [Required]
    [MaxLength(32)]
    public string SerialNumber { get; set; } = string.Empty;

    [Required]
    [MaxLength(32)]
    public string Status { get; set; } = string.Empty;

    [Required]
    public int TurbineTypeId { get; set; }
}
