using WindPowerSystemV5.Server.Data.DTOs;

namespace WindPowerSystemV5.Server.Services.Interfaces;

public interface IAiChatBotAgent
{
    Task<AiChatAnswerDTO> AskAboutTurbineTypes(string question, CancellationToken cancellationToken = default);
}
