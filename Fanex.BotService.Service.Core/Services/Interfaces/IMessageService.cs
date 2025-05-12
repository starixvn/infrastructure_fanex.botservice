namespace Fanex.BotService.Service.Core.Services.Interfaces;

using Fanex.BotService.Shared.Models;
using Models;

public interface IMessageService
{
    Task<int> InsertNewMessageAsync(Conversation message, int retryTimeSpan);

    Task UpdateMessageStatusAsync(SendMessageResult result, int retryTimeSpan);
}