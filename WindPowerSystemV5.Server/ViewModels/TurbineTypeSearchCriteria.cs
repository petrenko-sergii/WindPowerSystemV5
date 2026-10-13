namespace WindPowerSystemV5.Server.ViewModels;

public class TurbineTypeSearchCriteria
{
    public const int MaxLimit = 20;

    public const string SortByCapacity = "capacity";
    public const string SortByManufacturer = "manufacturer";
    public const string SortByModel = "model";
    public const string SortByTurbineQty = "turbineQty";

    /// <summary>Case-insensitive "contains" filter.</summary>
    public string? Manufacturer { get; set; }

    /// <summary>Case-insensitive "contains" filter.</summary>
    public string? Model { get; set; }

    public float? MinCapacity { get; set; }

    public float? MaxCapacity { get; set; }

    public int? MinTurbineQty { get; set; }

    /// <summary>One of the SortBy* constants; unknown or empty values sort by Id.</summary>
    public string? SortBy { get; set; }

    public bool SortDescending { get; set; }

    /// <summary>Clamped to 1..<see cref="MaxLimit"/>.</summary>
    public int Limit { get; set; } = MaxLimit;
}
