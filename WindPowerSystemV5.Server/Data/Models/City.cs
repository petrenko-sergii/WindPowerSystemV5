using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace WindPowerSystemV5.Server.Data.Models;

[Table("Cities")]
[Index(nameof(Name))]
[Index(nameof(Lat))]
[Index(nameof(Lon))]
public class City
{
    [Key]
    [Required]
    public int Id { get; set; }

    public required string Name { get; set; }

    [Column(TypeName = "decimal(7,4)")]
    public decimal Lat { get; set; }

    [Column(TypeName = "decimal(7,4)")]
    public decimal Lon { get; set; }

    [ForeignKey(nameof(Country))]
    public int CountryId { get; set; }

    public Country? Country { get; set; }
}
