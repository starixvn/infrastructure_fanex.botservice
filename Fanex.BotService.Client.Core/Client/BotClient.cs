namespace Fanex.BotService.Client.Core.Client
{
    using System.Threading.Tasks;
    using Fanex.BotService.Client.Core.Models;

    internal interface IBotClient
    {
        Task SendTelegramAsync(Conversation conversation);

        Task SendSkypeAsync(Conversation conversation);

        Task SendAsync(Conversation conversation);
    }

    internal class BotClient : BaseHttpClient, IBotClient
    {
        public async Task SendSkypeAsync(Conversation conversation)
            => await PostAsync<Conversation, Task>(ApiPath.SkypeChannelApiPath, conversation);

        public async Task SendTelegramAsync(Conversation conversation)
            => await PostAsync<Conversation, Task>(ApiPath.TelegramChannelApiPath, conversation);

        public async Task SendAsync(Conversation conversation)
        => await PostAsync<Conversation, Task>(ApiPath.BotApiPath, conversation);

        private static class ApiPath
        {
            public const string SkypeChannelApiPath = "api/skype/send";
            public const string TelegramChannelApiPath = "api/telegram/send";
            public const string BotApiPath = "api/bot/send";
        }
    }
}