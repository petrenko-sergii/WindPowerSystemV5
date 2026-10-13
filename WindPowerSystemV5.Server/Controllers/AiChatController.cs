using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WindPowerSystemV5.Server.Data.DTOs;
using WindPowerSystemV5.Server.Services.Interfaces;
using WindPowerSystemV5.Server.ViewModels;

namespace WindPowerSystemV5.Server.Controllers;

[Route("api/ai-chat")]
[ApiController]
public class AiChatController : ControllerBase
{
    private readonly IAiChatBotAgent _aiChatBotAgent;

    public AiChatController(IAiChatBotAgent aiChatBotAgent)
    {
        _aiChatBotAgent = aiChatBotAgent;
    }

    // Each call spends Anthropic API tokens, so it is limited to authenticated users.
    [Authorize]
    [HttpPost("turbine-types")]
    public async Task<ActionResult<AiChatAnswerDTO>> AskAboutTurbineTypes(
        [FromBody] AiChatRequest request,
        CancellationToken cancellationToken)
    {
        return await _aiChatBotAgent.AskAboutTurbineTypes(request.Question, cancellationToken);
    }
}
