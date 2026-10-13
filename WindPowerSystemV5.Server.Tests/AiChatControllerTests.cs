using Microsoft.AspNetCore.Authorization;
using NSubstitute;
using WindPowerSystemV5.Server.Controllers;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Services.Interfaces;
using WindPowerSystemV5.Server.ViewModels;

namespace WindPowerSystemV5.Server.Tests;

public class AiChatControllerTests
{
    [Fact]
    public async Task AskAboutTurbineTypes_PassesQuestionToAgentAndReturnsResult()
    {
        // Arrange
        var agent = Substitute.For<IAiChatBotAgent>();
        var expected = new AiChatAnswerDTO { Answer = "Two types." };
        using var cts = new CancellationTokenSource();
        agent.AskAboutTurbineTypes("Vestas types?", cts.Token).Returns(expected);
        var controller = new AiChatController(agent);

        // Act
        var result = await controller.AskAboutTurbineTypes(new AiChatRequest { Question = "Vestas types?" }, cts.Token);

        // Assert
        Assert.Same(expected, result.Value);
        await agent.Received(1).AskAboutTurbineTypes("Vestas types?", cts.Token);
    }

    [Fact]
    public void AskAboutTurbineTypes_RequiresAuthorization()
    {
        // Arrange
        var method = typeof(AiChatController).GetMethod(nameof(AiChatController.AskAboutTurbineTypes))!;

        // Act
        var attributes = method.GetCustomAttributes(typeof(AuthorizeAttribute), true);

        // Assert
        Assert.NotEmpty(attributes);
    }
}
