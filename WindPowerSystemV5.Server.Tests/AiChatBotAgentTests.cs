using System.Text.Json;
using Anthropic;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using WindPowerSystemV5.Server.Config;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Services;
using WindPowerSystemV5.Server.Services.Interfaces;
using WindPowerSystemV5.Server.Utils.Exceptions;
using WindPowerSystemV5.Server.ViewModels;

namespace WindPowerSystemV5.Server.Tests;

public class AiChatBotAgentTests
{
    private static readonly TurbineTypeDTO Vestas = new()
    {
        Id = 1, Manufacturer = "Vestas", Model = "V39/600", Capacity = 600, TurbineQty = 2
    };

    private static readonly TurbineTypeDTO Enercon = new()
    {
        Id = 4, Manufacturer = "Enercon", Model = "E-44", Capacity = 900, TurbineQty = 0
    };

    private readonly ITurbineTypeService _turbineTypeService = Substitute.For<ITurbineTypeService>();

    private AiChatBotAgent CreateAgent(StubHttpMessageHandler handler)
    {
        var client = new AnthropicClient
        {
            ApiKey = "test-key",
            BaseUrl = "https://anthropic.test",
            MaxRetries = 0,
            HttpClient = new HttpClient(handler)
        };
        var options = Options.Create(new AnthropicOptions { Model = "test-model" });
        return new AiChatBotAgent(client, options, _turbineTypeService, NullLogger<AiChatBotAgent>.Instance);
    }

    private static string MessageJson(string stopReason, string contentJson) =>
        $$$"""
        {"id":"msg_1","type":"message","role":"assistant","model":"test-model","stop_reason":"{{{stopReason}}}","stop_sequence":null,
         "content":[{{{contentJson}}}],
         "usage":{"input_tokens":1,"output_tokens":1}}
        """;

    private static string ToolUse(string name = "search_turbine_types", string inputJson = """{"manufacturer":"Vestas"}""") =>
        $$$"""{"type":"tool_use","id":"toolu_1","name":"{{{name}}}","input":{{{inputJson}}}}""";

    private static string Text(string text) =>
        $$$"""{"type":"text","text":{{{JsonSerializer.Serialize(text)}}}}""";

    private static string AnswerJson(string answer, params int[] ids) =>
        JsonSerializer.Serialize(new { answer, turbineTypeIds = ids });

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task AskAboutTurbineTypes_BlankQuestion_ThrowsBadRequest(string question)
    {
        // Arrange
        var handler = new StubHttpMessageHandler();
        var agent = CreateAgent(handler);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => agent.AskAboutTurbineTypes(question));
        Assert.Empty(handler.RequestedUrls);
    }

    [Fact]
    public async Task AskAboutTurbineTypes_QuestionTooLong_ThrowsBadRequest()
    {
        // Arrange
        var handler = new StubHttpMessageHandler();
        var agent = CreateAgent(handler);
        var question = new string('a', AiChatBotAgent.MaxQuestionLength + 1);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => agent.AskAboutTurbineTypes(question));
        Assert.Empty(handler.RequestedUrls);
    }

    [Fact]
    public async Task AskAboutTurbineTypes_QuestionWithControlChars_ThrowsBadRequest()
    {
        // Arrange
        var handler = new StubHttpMessageHandler();
        var agent = CreateAgent(handler);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => agent.AskAboutTurbineTypes("Vestas\u0000types"));
        Assert.Empty(handler.RequestedUrls);
    }

    [Fact]
    public async Task AskAboutTurbineTypes_ToolCallThenAnswer_ReturnsAnswerWithUrls()
    {
        // Arrange
        _turbineTypeService
            .Search(Arg.Is<TurbineTypeSearchCriteria>(c => c.Manufacturer == "Vestas"))
            .Returns([Vestas]);
        var handler = new StubHttpMessageHandler()
            .Enqueue(MessageJson("tool_use", ToolUse()))
            .Enqueue(MessageJson("end_turn", Text(AnswerJson("Vestas has one type.", 1))));
        var agent = CreateAgent(handler);

        // Act
        var result = await agent.AskAboutTurbineTypes("Which Vestas turbine types exist?");

        // Assert
        Assert.Equal(2, handler.RequestedUrls.Count);
        Assert.Equal("Vestas has one type.", result.Answer);
        var turbineType = Assert.Single(result.TurbineTypes);
        Assert.Equal(1, turbineType.Id);
        Assert.Equal("Vestas", turbineType.Manufacturer);
        Assert.Equal("V39/600", turbineType.Model);
        Assert.Equal(600, turbineType.Capacity);
        Assert.Equal(2, turbineType.TurbineQty);
    }

    [Fact]
    public async Task AskAboutTurbineTypes_ToolInputIsMappedToSearchCriteria()
    {
        // Arrange
        _turbineTypeService.Search(Arg.Any<TurbineTypeSearchCriteria>()).Returns([Enercon]);
        var handler = new StubHttpMessageHandler()
            .Enqueue(MessageJson("tool_use", ToolUse(inputJson:
                """{"minCapacity":800,"maxCapacity":1000.5,"minTurbineQty":0,"sortBy":"capacity","sortDescending":true,"limit":3}""")))
            .Enqueue(MessageJson("end_turn", Text(AnswerJson("One match.", 4))));
        var agent = CreateAgent(handler);

        // Act
        await agent.AskAboutTurbineTypes("Types between 800 and 1000 kW?");

        // Assert
        await _turbineTypeService.Received(1).Search(Arg.Is<TurbineTypeSearchCriteria>(c =>
            c.Manufacturer == null &&
            c.MinCapacity == 800f &&
            c.MaxCapacity == 1000.5f &&
            c.MinTurbineQty == 0 &&
            c.SortBy == "capacity" &&
            c.SortDescending &&
            c.Limit == 3));
    }

    [Fact]
    public async Task AskAboutTurbineTypes_ToolThrows_ReportsErrorToClaudeAndStillAnswers()
    {
        // Arrange
        _turbineTypeService.Search(Arg.Any<TurbineTypeSearchCriteria>(), Arg.Any<CancellationToken>())
            .Returns<List<TurbineTypeDTO>>(_ => throw new InvalidOperationException("database is down"));
        var handler = new StubHttpMessageHandler()
            .Enqueue(MessageJson("tool_use", ToolUse()))
            .Enqueue(MessageJson("end_turn", Text(AnswerJson("I could not read the data right now."))));
        var agent = CreateAgent(handler);

        // Act
        var result = await agent.AskAboutTurbineTypes("Which Vestas types do we have?");

        // Assert
        Assert.Equal("I could not read the data right now.", result.Answer);
        Assert.Equal(2, handler.RequestedUrls.Count);
        Assert.Contains("\"is_error\":true", handler.RequestBodies[1]);
        Assert.DoesNotContain("database is down", handler.RequestBodies[1]);
    }

    [Fact]
    public async Task AskAboutTurbineTypes_ToolCancelled_PropagatesCancellation()
    {
        // Arrange
        _turbineTypeService.Search(Arg.Any<TurbineTypeSearchCriteria>(), Arg.Any<CancellationToken>())
            .Returns<List<TurbineTypeDTO>>(_ => throw new OperationCanceledException());
        var handler = new StubHttpMessageHandler()
            .Enqueue(MessageJson("tool_use", ToolUse()));
        var agent = CreateAgent(handler);

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(
            () => agent.AskAboutTurbineTypes("Which Vestas types do we have?"));
    }

    [Fact]
    public async Task AskAboutTurbineTypes_CancellationToken_IsPassedToDataLayer()
    {
        // Arrange
        using var cts = new CancellationTokenSource();
        _turbineTypeService.Search(Arg.Any<TurbineTypeSearchCriteria>(), Arg.Any<CancellationToken>())
            .Returns([Vestas]);
        var handler = new StubHttpMessageHandler()
            .Enqueue(MessageJson("tool_use", ToolUse()))
            .Enqueue(MessageJson("end_turn", Text(AnswerJson("Vestas V39/600.", 1))));
        var agent = CreateAgent(handler);

        // Act
        await agent.AskAboutTurbineTypes("Which Vestas types do we have?", cts.Token);

        // Assert
        await _turbineTypeService.Received(1).Search(Arg.Any<TurbineTypeSearchCriteria>(), cts.Token);
    }

    [Fact]
    public async Task AskAboutTurbineTypes_ResultShorterThanLimit_DoesNotRunCountQuery()
    {
        // Arrange
        _turbineTypeService.Search(Arg.Any<TurbineTypeSearchCriteria>()).Returns([Vestas]);
        var handler = new StubHttpMessageHandler()
            .Enqueue(MessageJson("tool_use", ToolUse(inputJson: """{"manufacturer":"Vestas","limit":5}""")))
            .Enqueue(MessageJson("end_turn", Text(AnswerJson("Vestas V39/600.", 1))));
        var agent = CreateAgent(handler);

        // Act
        await agent.AskAboutTurbineTypes("Which Vestas types do we have?");

        // Assert
        await _turbineTypeService.DidNotReceiveWithAnyArgs().Count(default!);
    }

    [Fact]
    public async Task AskAboutTurbineTypes_ResultReachesLimit_RunsCountQuery()
    {
        // Arrange
        _turbineTypeService.Search(Arg.Any<TurbineTypeSearchCriteria>()).Returns([Vestas]);
        _turbineTypeService.Count(Arg.Any<TurbineTypeSearchCriteria>()).Returns(4);
        var handler = new StubHttpMessageHandler()
            .Enqueue(MessageJson("tool_use", ToolUse(inputJson: """{"limit":1}""")))
            .Enqueue(MessageJson("end_turn", Text(AnswerJson("One of several.", 1))));
        var agent = CreateAgent(handler);

        // Act
        await agent.AskAboutTurbineTypes("Show me a turbine type");

        // Assert
        await _turbineTypeService.Received(1).Count(Arg.Any<TurbineTypeSearchCriteria>());
    }

    [Fact]
    public async Task AskAboutTurbineTypes_CountryQuestion_ListsManufacturersThenSearchesMatchingOnes()
    {
        // Arrange
        _turbineTypeService.GetManufacturers().Returns(
        [
            new ManufacturerSummaryDTO { Manufacturer = "Enercon", TurbineTypeCount = 1 },
            new ManufacturerSummaryDTO { Manufacturer = "Vestas", TurbineTypeCount = 1 }
        ]);
        _turbineTypeService
            .Search(Arg.Is<TurbineTypeSearchCriteria>(c => c.Manufacturer == "Vestas"))
            .Returns([Vestas]);
        var handler = new StubHttpMessageHandler()
            .Enqueue(MessageJson("tool_use", ToolUse(name: "list_manufacturers", inputJson: "{}")))
            .Enqueue(MessageJson("tool_use", ToolUse()))
            .Enqueue(MessageJson("end_turn", Text(AnswerJson("Vestas is Danish (general knowledge).", 1))));
        var agent = CreateAgent(handler);

        // Act
        var result = await agent.AskAboutTurbineTypes("Which turbine types are produced in Denmark?");

        // Assert
        Assert.Equal(3, handler.RequestedUrls.Count);
        await _turbineTypeService.Received(1).GetManufacturers();
        var turbineType = Assert.Single(result.TurbineTypes);
        Assert.Equal("Vestas", turbineType.Manufacturer);
    }

    [Fact]
    public async Task AskAboutTurbineTypes_AnswerWithUnknownId_DropsUnknownId()
    {
        // Arrange
        _turbineTypeService.Search(Arg.Any<TurbineTypeSearchCriteria>()).Returns([Vestas]);
        var handler = new StubHttpMessageHandler()
            .Enqueue(MessageJson("tool_use", ToolUse()))
            .Enqueue(MessageJson("end_turn", Text(AnswerJson("Two types.", 1, 999, 1))));
        var agent = CreateAgent(handler);

        // Act
        var result = await agent.AskAboutTurbineTypes("Vestas types?");

        // Assert
        var turbineType = Assert.Single(result.TurbineTypes);
        Assert.Equal(1, turbineType.Id);
    }

    [Fact]
    public async Task AskAboutTurbineTypes_OffTopicQuestionWithoutToolCall_ReturnsAnswerWithEmptyList()
    {
        // Arrange
        var handler = new StubHttpMessageHandler()
            .Enqueue(MessageJson("end_turn", Text(AnswerJson("I can only help with turbine types."))));
        var agent = CreateAgent(handler);

        // Act
        var result = await agent.AskAboutTurbineTypes("What is the capital of France?");

        // Assert
        Assert.Equal("I can only help with turbine types.", result.Answer);
        Assert.Empty(result.TurbineTypes);
        await _turbineTypeService.DidNotReceiveWithAnyArgs().Search(default!);
    }

    [Fact]
    public async Task AskAboutTurbineTypes_Refusal_ThrowsBadRequest()
    {
        // Arrange
        var handler = new StubHttpMessageHandler().Enqueue(MessageJson("refusal", Text("No")));
        var agent = CreateAgent(handler);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => agent.AskAboutTurbineTypes("Vestas types?"));
    }

    [Fact]
    public async Task AskAboutTurbineTypes_MaxTokensStop_ThrowsClearErrorWithoutParsing()
    {
        // Arrange
        var handler = new StubHttpMessageHandler()
            .Enqueue(MessageJson("max_tokens", Text("""{"answer":"Vestas V39""")));
        var agent = CreateAgent(handler);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => agent.AskAboutTurbineTypes("Vestas types?"));
        Assert.Contains("cut off", ex.Message);
    }

    [Fact]
    public async Task AskAboutTurbineTypes_MaxTokensStopInsideToolUse_DoesNotRunTool()
    {
        // Arrange
        var handler = new StubHttpMessageHandler()
            .Enqueue(MessageJson("max_tokens", ToolUse(inputJson: "{}")));
        var agent = CreateAgent(handler);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => agent.AskAboutTurbineTypes("Vestas types?"));
        await _turbineTypeService.DidNotReceiveWithAnyArgs().Search(default!);
    }

    [Fact]
    public async Task AskAboutTurbineTypes_InvalidJsonAnswer_ThrowsClearError()
    {
        // Arrange
        var handler = new StubHttpMessageHandler()
            .Enqueue(MessageJson("end_turn", Text("this is not json")));
        var agent = CreateAgent(handler);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => agent.AskAboutTurbineTypes("Vestas types?"));
        Assert.Contains("not valid JSON", ex.Message);
    }

    [Fact]
    public async Task AskAboutTurbineTypes_UnknownTool_DoesNotSearch()
    {
        // Arrange
        var handler = new StubHttpMessageHandler()
            .Enqueue(MessageJson("tool_use", ToolUse(name: "other_tool")))
            .Enqueue(MessageJson("end_turn", Text(AnswerJson("Nothing found."))));
        var agent = CreateAgent(handler);

        // Act
        var result = await agent.AskAboutTurbineTypes("Vestas types?");

        // Assert
        Assert.Equal(2, handler.RequestedUrls.Count);
        Assert.Empty(result.TurbineTypes);
        await _turbineTypeService.DidNotReceiveWithAnyArgs().Search(default!);
    }

    [Fact]
    public async Task AskAboutTurbineTypes_AgentNeverStopsCallingTools_ThrowsAfterMaxIterations()
    {
        // Arrange
        _turbineTypeService.Search(Arg.Any<TurbineTypeSearchCriteria>()).Returns([Vestas]);
        var handler = new StubHttpMessageHandler();
        for (var i = 0; i < 5; i++)
        {
            handler.Enqueue(MessageJson("tool_use", ToolUse()));
        }
        var agent = CreateAgent(handler);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => agent.AskAboutTurbineTypes("Vestas types?"));
        Assert.Equal(5, handler.RequestedUrls.Count);
    }
}
