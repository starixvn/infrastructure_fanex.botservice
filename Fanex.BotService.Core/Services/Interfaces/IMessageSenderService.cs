namespace Fanex.BotService.Core.Services.Interfaces
{
    using Fanex.BotService.Shared.Models;
    using Models;
    using System.Threading.Tasks;

    public interface IMessageSenderService
    {
        Task<SendMessageResult> SendMessageAsync(ConversationMessage conversationMessage);
    }
}