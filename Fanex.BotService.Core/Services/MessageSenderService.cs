namespace Fanex.NCB.BotService.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NCB.BotService.Core.Models;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Newtonsoft.Json;

    public class MessageSenderService : IMessageSenderService
    {
        private readonly HttpClient httpClient;
        private readonly AppSettings appSettings;
        private readonly ILogger<MessageSenderService> filelogger;
        private readonly Logging.ILogger logger;
        private readonly IBotFrameworkHttpAdapter adapter;

        public MessageSenderService(
            IHttpClientFactory httpClientFactory,
            AppSettings appSettings,
            IOptions<ProxySettings> proxySettings,
            ILogger<MessageSenderService> filelogger,
            Logging.ILogger logger,
            IBotFrameworkHttpAdapter adapter)

        {
            this.appSettings = appSettings;
            this.filelogger = filelogger;
            this.logger = logger;
            this.adapter = adapter;

            if (proxySettings.Value.IsUseProxy)
            {
                httpClient = httpClientFactory.CreateClient(NamedHttpClients.ProxiedClient);
                httpClient.Timeout = TimeSpan.FromSeconds(proxySettings.Value.TimeOut);
            }
            else
            {
                httpClient = httpClientFactory.CreateClient();
            }
        }

        public async Task<SendMessageResult> SendMessageAsync(ConversationMessage conversationMessage)
        {
            SendMessageResult sendMessageResult;
            if (appSettings.IsOnlySendIndirect)
            {
                sendMessageResult = await SendIndirectMessageAsync(conversationMessage);
            }
            else
            {
                sendMessageResult = await SendDirectMessageAsync(conversationMessage);      
            }

            return sendMessageResult;
        }

        private async Task<SendMessageResult> SendDirectMessageAsync(ConversationMessage conversationMessage)
        {
            var sendMessageResult = new SendMessageResult();
            try
            {
                var coversation = new ConversationReference
                {
                    Bot = new ChannelAccount { Id = conversationMessage.ConversationBotId, Name = conversationMessage.BotName, Role = "bot" },
                    Conversation = new ConversationAccount { Id = conversationMessage.ConversationId },
                    ServiceUrl = conversationMessage.ServiceURL,
                    ChannelId = conversationMessage.ChannelId
                };

                await ((BotAdapter)adapter).ContinueConversationAsync(conversationMessage.MicrosoftAppId, coversation,
                    async (context, token) =>
                    {
                        await context.SendActivityAsync(conversationMessage.Message, cancellationToken: token);
                    },
                    new CancellationToken());

                sendMessageResult.MessageId = conversationMessage.MessageId;
                sendMessageResult.ConversationId = conversationMessage.ConversationId;
                sendMessageResult.SentTime = DateTimeOffset.UtcNow.DateTime;
            }
            catch (Exception exception)
            {
                await WriteLogAsync(conversationMessage, exception);
                sendMessageResult = new SendMessageResult
                {
                    ErrorCode = 1,
                    ErrorMessage = exception.Message,
                    ConversationId = conversationMessage.ConversationId,
                    SentTime = DateTimeOffset.UtcNow.DateTime
                };
            }

            return sendMessageResult;
        }

        private async Task<SendMessageResult> SendIndirectMessageAsync(ConversationMessage conversationMessage)
        {
            try
            {
                var token = await GetTokenAsync();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
                var url = new Uri($"{appSettings.FowardUrl}");
                HttpResponseMessage response = await httpClient.PostAsJsonAsync(url, conversationMessage);
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                var sendMessageResult = JsonConvert.DeserializeObject<SendMessageResult>(responseData);
                sendMessageResult.SentTime = DateTimeOffset.UtcNow.DateTime;

                return sendMessageResult;
            }
            catch (Exception exception)
            {
                var logMessage = new LogMessage(exception.Message)
                    .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception));
                filelogger.LogWarning(logMessage.ToString());

                return new SendMessageResult
                {
                    ErrorCode = 1,
                    ErrorMessage = exception.Message,
                    ConversationId = conversationMessage.ConversationId,
                    SentTime = DateTimeOffset.UtcNow.DateTime
                };
            }
        }

        private async Task<ChatbotToken> GetTokenAsync()
        {
            var url = new Uri($"{appSettings.AuthenticateUrl}");
            var authData = new Dictionary<string, string>
            {
                { "username", appSettings.AppCredentialId },
                { "password", appSettings.AppCredentialPassword }
            };
            using var authDataContent = new FormUrlEncodedContent(authData);
            HttpResponseMessage response = await httpClient.PostAsync(url, authDataContent);
            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<ChatbotToken>(responseData);
            return token;
        }

        private async Task WriteLogAsync(ConversationMessage conversationMessage, Exception exception)
        {
            var param = new Dictionary<string, object>
            {
                { "channelId", conversationMessage.ChannelId },
                { "BotName", conversationMessage.BotName },
                { "chatId", conversationMessage.ConversationId },
                { "messageContent", conversationMessage.Message }
            };
            var logMessage = new LogMessage(exception.Message)
                .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                .AddSection("Params", param);

            await logger.ErrorAsync(logMessage.ToString());
        }
    }
}