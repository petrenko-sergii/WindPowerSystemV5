using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WindPowerSystemV5.Server.Data.Enums;

namespace WindPowerSystemV5.Server.Data.Models;

public class Turbine
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(32)]
    public string SerialNumber { get; set; } = string.Empty;

    [Required]
    //[MaxLength(32)]
    public TurbineStatus Status { get; set; }

    [Required]
    public int TurbineTypeId { get; set; }

    [ForeignKey(nameof(TurbineTypeId))]
    public TurbineType? TurbineType { get; set; }
}

public class TurbineConfiguration : IEntityTypeConfiguration<Turbine>
{
    public void Configure(EntityTypeBuilder<Turbine> builder)
    {
        builder.Property(t => t.Status)
               .HasConversion<string>();
    }
}
