namespace WindPowerSystemV5.Server.Data.DTOs;

public class CurrentWeatherDTO
{
    public string City { get; set; } = null!;
    public string? Country { get; set; }
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public decimal TemperatureC { get; set; }
    public decimal FeelsLikeC { get; set; }
    public int HumidityPercent { get; set; }
    public decimal PrecipitationMm { get; set; }
    public decimal WindSpeedKmh { get; set; }
    public string Conditions { get; set; } = null!;
}

public class ClothingItemDTO
{
    /// <summary>One of: head, top, bottom, outerwear, footwear, accessories.</summary>
    public string Category { get; set; } = null!;
    public string Item { get; set; } = null!;
    public string Reason { get; set; } = null!;
}

public class ClothingAdviceDTO
{
    public string Summary { get; set; } = null!;
    public List<ClothingItemDTO> Items { get; set; } = new();
    public bool UmbrellaNeeded { get; set; }
    public bool SunProtectionNeeded { get; set; }
}

public class WeatherClothingDTO
{
    public CurrentWeatherDTO Weather { get; set; } = null!;
    public ClothingAdviceDTO Advice { get; set; } = null!;
}
