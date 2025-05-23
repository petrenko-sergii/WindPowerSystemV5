﻿using System.Text.Json.Serialization;

namespace WindPowerSystemV5.Server.Data.DTOs;

public class CountryDTO
{
    public int Id { get; set; }

    public required string Name { get; set; }

    [JsonPropertyName("iso2")]
    public required string ISO2 { get; set; }

    [JsonPropertyName("iso3")]
    public required string ISO3 { get; set; }

    public int? CityQty { get; set; }
}
