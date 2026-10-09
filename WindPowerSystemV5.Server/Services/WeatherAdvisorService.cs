using System.Text.Json;
using Anthropic;
using Anthropic.Models.Messages;
using Microsoft.Extensions.Options;
using WindPowerSystemV5.Server.Config;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Services.Interfaces;
using WindPowerSystemV5.Server.Utils.Exceptions;

namespace WindPowerSystemV5.Server.Services;

/// <summary>
/// Claude agent: calls the get_weather tool for the requested city and returns structured clothing advice.
/// </summary>
public class WeatherAdvisorService : IWeatherAdvisorService
{
    private const string GetWeatherToolName = "get_weather";
    private const int MaxAgentIterations = 5;

    private const string SystemPrompt =
        "You are a clothing advisor. Always call the get_weather tool for the requested city first, " +
        "then recommend what to wear based on the returned data (use feels-like temperature, wind, " +
        "precipitation and conditions). Be concise and practical: at most 6 items, one short reason each.";

    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly AnthropicClient _client;
    private readonly AnthropicOptions _options;
    private readonly IWeatherLookupService _weatherLookup;

    public WeatherAdvisorService(
        AnthropicClient client,
        IOptions<AnthropicOptions> options,
        IWeatherLookupService weatherLookup)
    {
        _client = client;
        _options = options.Value;
        _weatherLookup = weatherLookup;
    }

    public async Task<WeatherClothingDTO> GetClothingAdvice(
        string city, decimal? lat = null, decimal? lon = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(city))
        {
            throw new BadRequestException("City is required.");
        }

        if (lat.HasValue != lon.HasValue)
        {
            throw new BadRequestException("Latitude and longitude must be passed together.");
        }

        CurrentWeatherDTO? weather = null;
        List<MessageParam> messages =
        [
            new() { Role = Role.User, Content = $"What should I wear in {city.Trim()} today?" }
        ];

        // The agent loop: send messages, run any requested tools, repeat until Claude stops asking for tools.
        for (var i = 0; i < MaxAgentIterations; i++)
        {
            var response = await _client.Messages.Create(new MessageCreateParams
            {
                Model = _options.Model,
                MaxTokens = 2048,
                System = SystemPrompt,
                Tools = [BuildWeatherTool()],
                OutputConfig = new OutputConfig
                {
                    Effort = Effort.Low,
                    Format = BuildAdviceFormat()
                },
                Messages = messages
            }, cancellationToken);

            if (response.StopReason == "refusal")
            {
                throw new BadRequestException("The request was declined.");
            }

            List<ContentBlockParam> assistantContent = [];
            List<ContentBlockParam> toolResults = [];
            string? finalText = null;

            foreach (ContentBlock block in response.Content)
            {
                if (block.TryPickText(out TextBlock? text))
                {
                    assistantContent.Add(new TextBlockParam { Text = text.Text });
                    finalText = text.Text;
                }
                else if (block.TryPickThinking(out ThinkingBlock? thinking))
                {
                    assistantContent.Add(new ThinkingBlockParam
                    {
                        Thinking = thinking.Thinking,
                        Signature = thinking.Signature
                    });
                }
                else if (block.TryPickRedactedThinking(out RedactedThinkingBlock? redacted))
                {
                    assistantContent.Add(new RedactedThinkingBlockParam { Data = redacted.Data });
                }
                else if (block.TryPickToolUse(out ToolUseBlock? toolUse))
                {
                    assistantContent.Add(new ToolUseBlockParam
                    {
                        ID = toolUse.ID,
                        Name = toolUse.Name,
                        Input = toolUse.Input
                    });

                    if (toolUse.Name != GetWeatherToolName)
                    {
                        toolResults.Add(new ToolResultBlockParam
                        {
                            ToolUseID = toolUse.ID,
                            Content = $"Unknown tool: {toolUse.Name}",
                            IsError = true
                        });
                        continue;
                    }

                    var requestedCity = toolUse.Input["city"].GetString() ?? city;
                    weather = await _weatherLookup.GetCurrentWeather(requestedCity, lat, lon, cancellationToken);
                    toolResults.Add(new ToolResultBlockParam
                    {
                        ToolUseID = toolUse.ID,
                        Content = JsonSerializer.Serialize(weather)
                    });
                }
            }

            if (toolResults.Count == 0)
            {
                if (weather is null || string.IsNullOrWhiteSpace(finalText))
                {
                    throw new InvalidOperationException("The agent finished without weather data or advice.");
                }

                var advice = JsonSerializer.Deserialize<ClothingAdviceDTO>(finalText, JsonOptions)
                    ?? throw new InvalidOperationException("The agent returned an empty advice.");

                return new WeatherClothingDTO { Weather = weather, Advice = advice };
            }

            // Echo the assistant turn and our tool results back so Claude can continue.
            messages =
            [
                .. messages,
                new() { Role = Role.Assistant, Content = assistantContent },
                new() { Role = Role.User, Content = toolResults }
            ];
        }

        throw new InvalidOperationException("The agent did not finish within the allowed number of steps.");
    }

    private static Tool BuildWeatherTool() => new()
    {
        Name = GetWeatherToolName,
        Description = "Get the current weather for a city.",
        InputSchema = new()
        {
            Properties = new Dictionary<string, JsonElement>
            {
                ["city"] = JsonSerializer.SerializeToElement(
                    new { type = "string", description = "The city to get weather for" })
            },
            Required = ["city"]
        }
    };

    private static JsonOutputFormat BuildAdviceFormat() => new()
    {
        Schema = new Dictionary<string, JsonElement>
        {
            ["type"] = JsonSerializer.SerializeToElement("object"),
            ["properties"] = JsonSerializer.SerializeToElement(new
            {
                summary = new { type = "string" },
                items = new
                {
                    type = "array",
                    items = new
                    {
                        type = "object",
                        properties = new
                        {
                            category = new
                            {
                                type = "string",
                                @enum = new[] { "head", "top", "bottom", "outerwear", "footwear", "accessories" }
                            },
                            item = new { type = "string" },
                            reason = new { type = "string" }
                        },
                        required = new[] { "category", "item", "reason" },
                        additionalProperties = false
                    }
                },
                umbrellaNeeded = new { type = "boolean" },
                sunProtectionNeeded = new { type = "boolean" }
            }),
            ["required"] = JsonSerializer.SerializeToElement(
                new[] { "summary", "items", "umbrellaNeeded", "sunProtectionNeeded" }),
            ["additionalProperties"] = JsonSerializer.SerializeToElement(false)
        }
    };
}
