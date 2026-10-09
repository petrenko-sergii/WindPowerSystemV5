using System.Net;
using WindPowerSystemV5.Server.Services;
using WindPowerSystemV5.Server.Utils.Exceptions;

namespace WindPowerSystemV5.Server.Tests;

public class OpenMeteoWeatherServiceTests
{
    private const string GeoJson =
        """{"results":[{"name":"Kyiv","latitude":50.45,"longitude":30.52,"country":"Ukraine"}]}""";

    private const string ForecastJson =
        """{"current":{"temperature_2m":12.5,"apparent_temperature":10.1,"relative_humidity_2m":70,"precipitation":0.4,"weather_code":61,"wind_speed_10m":15.2}}""";

    [Fact]
    public async Task GetCurrentWeather_WithoutCoordinates_GeocodesThenReturnsWeather()
    {
        var handler = new StubHttpMessageHandler().Enqueue(GeoJson).Enqueue(ForecastJson);
        var service = new OpenMeteoWeatherService(new HttpClient(handler));

        var result = await service.GetCurrentWeather("Kyiv");

        Assert.Equal(2, handler.RequestedUrls.Count);
        Assert.Contains("name=Kyiv", handler.RequestedUrls[0]);
        Assert.Equal("Kyiv", result.City);
        Assert.Equal("Ukraine", result.Country);
        Assert.Equal(50.45m, result.Latitude);
        Assert.Equal(30.52m, result.Longitude);
        Assert.Equal(12.5m, result.TemperatureC);
        Assert.Equal(10.1m, result.FeelsLikeC);
        Assert.Equal(70, result.HumidityPercent);
        Assert.Equal(0.4m, result.PrecipitationMm);
        Assert.Equal(15.2m, result.WindSpeedKmh);
        Assert.Equal("Rain", result.Conditions);
    }

    [Fact]
    public async Task GetCurrentWeather_WithCoordinates_SkipsGeocoding()
    {
        var handler = new StubHttpMessageHandler().Enqueue(ForecastJson);
        var service = new OpenMeteoWeatherService(new HttpClient(handler));

        var result = await service.GetCurrentWeather("Lviv", 49.84m, 24.03m);

        Assert.Single(handler.RequestedUrls);
        Assert.Contains("latitude=49.84&longitude=24.03", handler.RequestedUrls[0]);
        Assert.Equal("Lviv", result.City);
        Assert.Null(result.Country);
        Assert.Equal(49.84m, result.Latitude);
    }

    [Fact]
    public async Task GetCurrentWeather_CityNotFound_ThrowsNotFoundException()
    {
        var handler = new StubHttpMessageHandler().Enqueue("""{"results":[]}""");
        var service = new OpenMeteoWeatherService(new HttpClient(handler));

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetCurrentWeather("Nowhereville"));
    }

    [Fact]
    public async Task GetCurrentWeather_GeocodingResponseHasNoResultsProperty_ThrowsNotFoundException()
    {
        var handler = new StubHttpMessageHandler().Enqueue("{}");
        var service = new OpenMeteoWeatherService(new HttpClient(handler));

        await Assert.ThrowsAsync<NotFoundException>(() => service.GetCurrentWeather("Nowhereville"));
    }

    [Fact]
    public async Task GetCurrentWeather_UpstreamError_ThrowsHttpRequestException()
    {
        var handler = new StubHttpMessageHandler().Enqueue("{}", HttpStatusCode.InternalServerError);
        var service = new OpenMeteoWeatherService(new HttpClient(handler));

        await Assert.ThrowsAsync<HttpRequestException>(() => service.GetCurrentWeather("Kyiv", 1, 1));
    }

    [Theory]
    [InlineData(0, "Clear sky")]
    [InlineData(3, "Overcast")]
    [InlineData(45, "Fog")]
    [InlineData(53, "Drizzle")]
    [InlineData(63, "Rain")]
    [InlineData(73, "Snow")]
    [InlineData(81, "Rain showers")]
    [InlineData(86, "Snow showers")]
    [InlineData(95, "Thunderstorm")]
    [InlineData(99, "Thunderstorm with hail")]
    [InlineData(1234, "Unknown")]
    public async Task GetCurrentWeather_MapsWeatherCodeToDescription(int code, string expected)
    {
        var json = ForecastJson.Replace("\"weather_code\":61", $"\"weather_code\":{code}");
        var handler = new StubHttpMessageHandler().Enqueue(json);
        var service = new OpenMeteoWeatherService(new HttpClient(handler));

        var result = await service.GetCurrentWeather("X", 1, 1);

        Assert.Equal(expected, result.Conditions);
    }
}
