namespace Fanex.BotService.Comsumer.Services.Interfaces
{
    using Fanex.BotService.Service.Core.Models;
    using Fanex.NCB.Shared.Models;

    public interface IMessageReceiverService
    {
        Task ConversationConsumerHandlerAsync(Conversation conversation);

        Task UpdateMessageStatusResult(SendMessageResult result);
    }
}