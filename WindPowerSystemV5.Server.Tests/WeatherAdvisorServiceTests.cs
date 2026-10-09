using System.Text.Json;
using Anthropic;
using Microsoft.Extensions.Options;
using NSubstitute;
using WindPowerSystemV5.Server.Config;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Services;
using WindPowerSystemV5.Server.Services.Interfaces;
using WindPowerSystemV5.Server.Utils.Exceptions;

namespace WindPowerSystemV5.Server.Tests;

public class WeatherAdvisorServiceTests
{
    private const string AdviceJson =
        """{"summary":"Dress warmly","items":[{"category":"outerwear","item":"Rain jacket","reason":"Rainy"}],"umbrellaNeeded":true,"sunProtectionNeeded":false}""";

    private static readonly CurrentWeatherDTO Weather = new()
    {
        City = "Kyiv",
        Conditions = "Rain",
        TemperatureC = 10
    };

    private readonly IWeatherLookupService _weatherLookup = Substitute.For<IWeatherLookupService>();

    private WeatherAdvisorService CreateService(StubHttpMessageHandler handler)
    {
        var client = new AnthropicClient
        {
            ApiKey = "test-key",
            BaseUrl = "https://anthropic.test",
            MaxRetries = 0,
            HttpClient = new HttpClient(handler)
        };
        var options = Options.Create(new AnthropicOptions { Model = "test-model" });
        return new WeatherAdvisorService(client, options, _weatherLookup);
    }

    private static string MessageJson(string stopReason, string contentJson) =>
        $$$"""
        {"id":"msg_1","type":"message","role":"assistant","model":"test-model","stop_reason":"{{{stopReason}}}","stop_sequence":null,
         "content":[{{{contentJson}}}],
         "usage":{"input_tokens":1,"output_tokens":1}}
        """;

    private static string ToolUse(string name = "get_weather", string city = "Kyiv") =>
        $$$"""{"type":"tool_use","id":"toolu_1","name":"{{{name}}}","input":{"city":"{{{city}}}"}}""";

    private static string Text(string text) =>
        $$$"""{"type":"text","text":{{{JsonSerializer.Serialize(text)}}}}""";

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task GetClothingAdvice_BlankCity_ThrowsBadRequest(string city)
    {
        var service = CreateService(new StubHttpMessageHandler());

        await Assert.ThrowsAsync<BadRequestException>(() => service.GetClothingAdvice(city));
    }

    [Fact]
    public async Task GetClothingAdvice_OnlyOneCoordinate_ThrowsBadRequest()
    {
        var service = CreateService(new StubHttpMessageHandler());

        await Assert.ThrowsAsync<BadRequestException>(() => service.GetClothingAdvice("Kyiv", 50m, null));
        await Assert.ThrowsAsync<BadRequestException>(() => service.GetClothingAdvice("Kyiv", null, 30m));
    }

    [Fact]
    public async Task GetClothingAdvice_ToolCallThenAdvice_ReturnsWeatherAndAdvice()
    {
        _weatherLookup.GetCurrentWeather("Kyiv", 50m, 30m, Arg.Any<CancellationToken>()).Returns(Weather);
        var handler = new StubHttpMessageHandler()
            .Enqueue(MessageJson("tool_use", ToolUse()))
            .Enqueue(MessageJson("end_turn", Text(AdviceJson)));
        var service = CreateService(handler);

        var result = await service.GetClothingAdvice("Kyiv", 50m, 30m);

        Assert.Equal(2, handler.RequestedUrls.Count);
        Assert.Same(Weather, result.Weather);
        Assert.Equal("Dress warmly", result.Advice.Summary);
        Assert.True(result.Advice.UmbrellaNeeded);
        Assert.False(result.Advice.SunProtectionNeeded);
        var item = Assert.Single(result.Advice.Items);
        Assert.Equal("outerwear", item.Category);
        Assert.Equal("Rain jacket", item.Item);
        await _weatherLookup.Received(1).GetCurrentWeather("Kyiv", 50m, 30m, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task GetClothingAdvice_Refusal_ThrowsBadRequest()
    {
        var handler = new StubHttpMessageHandler().Enqueue(MessageJson("refusal", Text("No")));
        var service = CreateService(handler);

        await Assert.ThrowsAsync<BadRequestException>(() => service.GetClothingAdvice("Kyiv"));
    }

    [Fact]
    public async Task GetClothingAdvice_AdviceWithoutWeatherLookup_ThrowsInvalidOperation()
    {
        var handler = new StubHttpMessageHandler().Enqueue(MessageJson("end_turn", Text(AdviceJson)));
        var service = CreateService(handler);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetClothingAdvice("Kyiv"));
        await _weatherLookup.DidNotReceiveWithAnyArgs().GetCurrentWeather(default!, default, default, default);
    }

    [Fact]
    public async Task GetClothingAdvice_UnknownTool_DoesNotLookUpWeather()
    {
        // The unknown tool gets an error result; the agent then answers without any weather data.
        var handler = new StubHttpMessageHandler()
            .Enqueue(MessageJson("tool_use", ToolUse(name: "other_tool")))
            .Enqueue(MessageJson("end_turn", Text(AdviceJson)));
        var service = CreateService(handler);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetClothingAdvice("Kyiv"));
        Assert.Equal(2, handler.RequestedUrls.Count);
        await _weatherLookup.DidNotReceiveWithAnyArgs().GetCurrentWeather(default!, default, default, default);
    }

    [Fact]
    public async Task GetClothingAdvice_AgentNeverStopsCallingTools_ThrowsAfterMaxIterations()
    {
        _weatherLookup.GetCurrentWeather(Arg.Any<string>(), null, null, Arg.Any<CancellationToken>()).Returns(Weather);
        var handler = new StubHttpMessageHandler();
        for (var i = 0; i < 5; i++)
        {
            handler.Enqueue(MessageJson("tool_use", ToolUse()));
        }
        var service = CreateService(handler);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.GetClothingAdvice("Kyiv"));
        Assert.Equal(5, handler.RequestedUrls.Count);
    }
}
