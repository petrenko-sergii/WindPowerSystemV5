namespace WindPowerSystemV5.Server.Config;

public class AnthropicOptions
{
    public const string Placeholder = "override-this";

    /// <summary>
    /// When empty (or the placeholder), the SDK falls back to the ANTHROPIC_API_KEY environment variable.
    /// </summary>
    public string? ApiKey { get; set; }

    public string Model { get; set; } = "claude-sonnet-5-5";
}
