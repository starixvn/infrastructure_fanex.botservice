namespace Fanex.BotService.Client.Core.Services
{
    using Client;
    using Configuration;
    using Enum;
    using Interfaces;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class BotService : IBotService
    {
        private readonly IBotSettings botSettings;

        public BotService()
        {
            botSettings = BotClientManager.BotSettings;
        }

        public async Task SendSkypeAsync(string conversationId, string content)
            => await SendAsync(BotChannel.Skype, conversationId, content);

        public async Task SendTelegramAsync(string conversationId, string content)
            => await SendAsync(BotChannel.Telegram, conversationId, content);

        public async Task SendMsTeamsAsync(string conversationId, string content)
            => await SendAsync(BotChannel.Teams, conversationId, content);

        public async Task SendAsync(BotChannel? channel, string conversationId, string content)
        {
            ValidateInputConversation(channel, conversationId, content);
            var conversation = new Conversation
            {
                Channel = channel,
                AppId = botSettings.AppId,
                ConversationId = conversationId,
                Content = content
            };

            await BotClient.SendAsync(conversation);
        }

        public async Task SendMessageWithImagesAsync(BotChannel channel, string conversationId, string content,
            List<string> imageUrls = null)
        {
            ValidateImageConversation(imageUrls);
            ValidateInputConversation(channel, conversationId, content);
            var conversation = new Conversation
            {
                Channel = channel,
                AppId = botSettings.AppId,
                ConversationId = conversationId,
                Content = content,
                Images = imageUrls
            };

            await BotClient.SendAsync(conversation);
        }

        private static void ValidateInputConversation(BotChannel? channel, string conversationId, string content)
        {
            if (channel is null)
            {
                throw new ArgumentException("BotChannel cannot be null", nameof(channel));
            }

            if (string.IsNullOrWhiteSpace(conversationId))
            {
                throw new ArgumentException("ConversationId cannot be null or empty", nameof(conversationId));
            }

            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentException("Content cannot be null or empty", nameof(content));
            }
        }

        private static void ValidateImageConversation(List<string> imageUrls)
        {
            if (imageUrls is null || imageUrls.Count == 0)
            {
                throw new ArgumentException("ImageUrls cannot be null", nameof(imageUrls));
            }
        }
    }
}