namespace Fanex.BotService.Comsumer.Services.Interfaces
{
    using Fanex.BotService.Service.Core.Models;

    public interface IMessageReceiverService
    {
        Task ConversationConsumerHandlerAsync(Conversation conversation);
    }
}