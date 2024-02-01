namespace Fanex.NCB.BotService.Core.Services.Interfaces
{
    using System.Threading.Tasks;
    using Fanex.NCB.BotService.Core.Models;
    using Fanex.NCB.Shared.Models;

    public interface IMessageSenderService
    {
        Task<SendMessageResult> SendMessageAsync(ConversationMessage conversationMessage);
    }
}