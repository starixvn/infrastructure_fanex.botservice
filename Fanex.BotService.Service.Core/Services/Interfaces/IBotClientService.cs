namespace Fanex.BotService.Service.Core.Services.Interfaces;

using Enum;
using Fanex.BotService.Shared.Models;
using Fanex.BotService.Shared.Models.Message;

public interface IBotClientService
{
    Task<SendMessageResult> SendMessageAsync(BotChannel botChannel, MessageEndpoint message);
}