namespace Fanex.BotService.Client.Core.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.BotService.Client.Core.Enum;

    public interface IBotService
    {
        Task SendSkypeAsync(string conversationId, string content);

        Task SendTelegramAsync(string conversationId, string content);

        Task SendAsync(BotChannel channel, string conversationId, string content);

        Task SendMessageWithImagesAsync(BotChannel channel, string conversationId, string message, List<string> imageUrls = null);
    }
}