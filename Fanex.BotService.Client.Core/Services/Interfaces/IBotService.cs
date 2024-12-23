namespace Fanex.BotService.Client.Core.Services.Interfaces
{
    using Fanex.BotService.Client.Core.Enum;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IBotService
    {
        Task SendSkypeAsync(string conversationId, string content);

        Task SendTelegramAsync(string conversationId, string content);

        Task SendMsTeamsAsync(string conversationId, string content);

        Task SendAsync(BotChannel channel, string conversationId, string content);

        Task SendMessageWithImagesAsync(BotChannel channel, string conversationId, string message, List<string> imageUrls = null);
    }
}