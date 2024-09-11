namespace Fanex.BotService.Client.Core.Services
{
    using Fanex.BotService.Client.Core.Client;
    using Fanex.BotService.Client.Core.Configuration;
    using Fanex.BotService.Client.Core.Enum;
    using Fanex.BotService.Client.Core.Models;
    using Fanex.BotService.Client.Core.Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class BotService : IBotService
    {
        private readonly IBotClient botClient;
        private readonly IBotSettings botSettings;

        public BotService()
        {
            botClient = new BotClient();
            botSettings = BotClientManager.BotSettings;
        }

        public async Task SendSkypeAsync(string conversationId, string content)
        {
            Conversation conversation = new Conversation
            {
                AppId = botSettings.AppId,
                ConversationId = conversationId,
                Content = content,
                Channel = BotChannel.Skype
            };

            ValidateInputConversation(conversation);

            await botClient.SendSkypeAsync(conversation);
        }

        public async Task SendTelegramAsync(string conversationId, string content)
        {
            Conversation conversation = new Conversation
            {
                AppId = botSettings.AppId,
                ConversationId = conversationId,
                Content = content,
                Channel = BotChannel.Telegram
            };

            ValidateInputConversation(conversation);

            await botClient.SendTelegramAsync(conversation);
        }

        public async Task SendAsync(BotChannel channel, string conversationId, string content)
        {
            Conversation conversation = new Conversation
            {
                Channel = channel,
                AppId = botSettings.AppId,
                ConversationId = conversationId,
                Content = content
            };

            ValidateInputConversation(conversation);

            await botClient.SendAsync(conversation);
        }

        private static void ValidateInputConversation(Conversation conversation)
        {
            if (conversation.Channel is null)
            {
                throw new ArgumentNullException(nameof(conversation.Channel));
            }

            if (string.IsNullOrWhiteSpace(conversation.ConversationId))
            {
                throw new ArgumentNullException(nameof(conversation.ConversationId));
            }

            if (string.IsNullOrWhiteSpace(conversation.Content))
            {
                throw new ArgumentNullException(nameof(conversation.Content));
            }
        }

        public async Task SendMessageWithImagesAsync(BotChannel channel, string conversationId, string message, List<string> imageUrls = null)
        {
            Conversation conversation = new Conversation
            {
                Channel = channel,
                AppId = botSettings.AppId,
                ConversationId = conversationId,
                Content = message,
                Images = imageUrls
            };

            ValidateInputConversation(conversation);
            await botClient.SendAsync(conversation);
        }
    }
}