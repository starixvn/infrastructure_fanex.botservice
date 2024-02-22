namespace Fanex.BotService.Service.Core.Services.Interfaces
{
    using Fanex.BotService.Service.Core.Enum;
    using Fanex.BotService.Shared.Models.Message;
    using Fanex.NCB.Shared.Models;

    public interface IBotClientService
    {
        Task<SendMessageResult> SendMessageAsync(BotChannel botChannel, MessageEndpoint message);
    }
}