namespace Fanex.BotService.Core.Services.Interfaces
{
    using System.Threading.Tasks;

    public interface IMessageValidationService
    {
        Task<bool> IsInvalidSendMessageToBotService(string conversationId, string message);

        Task InsertMessageLogIntoCache(string conversationId, string messageContent, string guid);

        Task RemoveMessageLogOnCache(string guid);
    }
}