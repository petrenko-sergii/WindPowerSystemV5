using System.ComponentModel.DataAnnotations;

namespace WindPowerSystemV5.Server.ViewModels;

public class NewsCreationRequest
{
    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string Author { get; set; } = null!;

    public List<string> Chapters { get; set; } = [];
}
