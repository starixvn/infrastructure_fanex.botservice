namespace Fanex.BotService.Service.Core.Services.Interfaces
{
    using Fanex.BotService.Service.Core.Models;
    using Fanex.NCB.Shared.Models;

    public interface IMessageService
    {
        Task<int> InsertNewMessageAsync(Conversation message);

        Task<CommandResult> UpdateMessageStatusAsync(long messageId, string status);
    }
}