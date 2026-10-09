using Microsoft.AspNetCore.Authorization;
using NSubstitute;
using WindPowerSystemV5.Server.Controllers;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Services.Interfaces;

namespace WindPowerSystemV5.Server.Tests;

public class WeatherControllerTests
{
    [Fact]
    public async Task GetClothingAdvice_PassesArgumentsToServiceAndReturnsResult()
    {
        // Arrange
        var advisor = Substitute.For<IWeatherAdvisorService>();
        var expected = new WeatherClothingDTO
        {
            Weather = new CurrentWeatherDTO { City = "Copenhagen", Conditions = "Rain" },
            Advice = new ClothingAdviceDTO { Summary = "Take a coat", UmbrellaNeeded = true }
        };
        using var cts = new CancellationTokenSource();
        advisor.GetClothingAdvice("Copenhagen", 55.68m, 12.57m, cts.Token).Returns(expected);
        var controller = new WeatherController(advisor);

        // Act
        var result = await controller.GetClothingAdvice("Copenhagen", 55.68m, 12.57m, cts.Token);

        // Assert
        Assert.Same(expected, result.Value);
        await advisor.Received(1).GetClothingAdvice("Copenhagen", 55.68m, 12.57m, cts.Token);
    }

    [Fact]
    public void GetClothingAdvice_RequiresAuthorization()
    {
        // Arrange
        var method = typeof(WeatherController).GetMethod(nameof(WeatherController.GetClothingAdvice))!;

        // Act
        var attributes = method.GetCustomAttributes(typeof(AuthorizeAttribute), true);

        // Assert
        Assert.NotEmpty(attributes);
    }
}
