using System.Text.Json;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Services.Interfaces;
using WindPowerSystemV5.Server.Utils.Exceptions;

namespace WindPowerSystemV5.Server.Services;

/// <summary>
/// Free, key-less weather source: https://open-meteo.com
/// </summary>
public class OpenMeteoWeatherService : IWeatherLookupService
{
    private const string GeocodingUrl = "https://geocoding-api.open-meteo.com/v1/search";
    private const string ForecastUrl = "https://api.open-meteo.com/v1/forecast";

    private readonly HttpClient _httpClient;

    public OpenMeteoWeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<CurrentWeatherDTO> GetCurrentWeather(
        string city, decimal? lat = null, decimal? lon = null, CancellationToken cancellationToken = default)
    {
        string cityName = city;
        string? country = null;

        // Known coordinates make the geocoding request unnecessary.
        if (lat is null || lon is null)
        {
            var geoUrl = $"{GeocodingUrl}?name={Uri.EscapeDataString(city)}&count=1&language=en&format=json";
            using var geoDoc = await GetJson(geoUrl, cancellationToken);

            if (!geoDoc.RootElement.TryGetProperty("results", out var results) || results.GetArrayLength() == 0)
            {
                throw new NotFoundException($"City '{city}' was not found.");
            }

            var place = results[0];
            lat = place.GetProperty("latitude").GetDecimal();
            lon = place.GetProperty("longitude").GetDecimal();
            cityName = place.GetProperty("name").GetString()!;
            country = place.TryGetProperty("country", out var c) ? c.GetString() : null;
        }

        var forecastUrl = FormattableString.Invariant(
            $"{ForecastUrl}?latitude={lat}&longitude={lon}") +
            "&current=temperature_2m,apparent_temperature,relative_humidity_2m,precipitation,weather_code,wind_speed_10m" +
            "&wind_speed_unit=kmh&timezone=auto";
        using var weatherDoc = await GetJson(forecastUrl, cancellationToken);
        var current = weatherDoc.RootElement.GetProperty("current");

        return new CurrentWeatherDTO
        {
            City = cityName,
            Country = country,
            Latitude = lat.Value,
            Longitude = lon.Value,
            TemperatureC = current.GetProperty("temperature_2m").GetDecimal(),
            FeelsLikeC = current.GetProperty("apparent_temperature").GetDecimal(),
            HumidityPercent = current.GetProperty("relative_humidity_2m").GetInt32(),
            PrecipitationMm = current.GetProperty("precipitation").GetDecimal(),
            WindSpeedKmh = current.GetProperty("wind_speed_10m").GetDecimal(),
            Conditions = DescribeWeatherCode(current.GetProperty("weather_code").GetInt32())
        };
    }

    private async Task<JsonDocument> GetJson(string url, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();
        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        return await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);
    }

    // WMO weather interpretation codes
    private static string DescribeWeatherCode(int code) => code switch
    {
        0 => "Clear sky",
        1 => "Mainly clear",
        2 => "Partly cloudy",
        3 => "Overcast",
        45 or 48 => "Fog",
        >= 51 and <= 57 => "Drizzle",
        >= 61 and <= 67 => "Rain",
        >= 71 and <= 77 => "Snow",
        >= 80 and <= 82 => "Rain showers",
        85 or 86 => "Snow showers",
        95 => "Thunderstorm",
        96 or 99 => "Thunderstorm with hail",
        _ => "Unknown"
    };
}
