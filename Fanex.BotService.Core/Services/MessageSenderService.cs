namespace Fanex.BotService.Core.Services;

using Fanex.BotService.Shared.Models;
using Interfaces;
using Logging.Extensions;
using Logging.Extensions.Database;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

public class MessageSenderService : IMessageSenderService
{
    private readonly HttpClient _httpClient;
    private readonly AppSettings _appSettings;
    private readonly ILogger<MessageSenderService> _fileLogger;
    private readonly Logging.ILogger _logger;
    private readonly IBotFrameworkHttpAdapter _adapter;

    public MessageSenderService(
        IHttpClientFactory httpClientFactory,
        AppSettings appSettings,
        IOptions<ProxySettings> proxySettings,
        ILogger<MessageSenderService> fileLogger,
        Logging.ILogger logger,
        IBotFrameworkHttpAdapter adapter)

    {
        this._appSettings = appSettings;
        this._fileLogger = fileLogger;
        this._logger = logger;
        this._adapter = adapter;

        if (proxySettings.Value.IsUseProxy)
        {
            _httpClient = httpClientFactory.CreateClient(NamedHttpClients.ProxiedClient);
            _httpClient.Timeout = TimeSpan.FromSeconds(proxySettings.Value.TimeOut);
        }
        else
        {
            _httpClient = httpClientFactory.CreateClient();
        }
    }

    public async Task<SendMessageResult> SendMessageAsync(ConversationMessage conversationMessage)
    {
        SendMessageResult sendMessageResult;
        if (_appSettings.IsOnlySendIndirect)
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
            sendMessageResult.MessageId = conversationMessage.MessageId;
            sendMessageResult.ConversationId = conversationMessage.ConversationId;
            var conversation = new ConversationReference
            {
                Bot = new ChannelAccount { Id = conversationMessage.ConversationBotId, Name = conversationMessage.BotName, Role = "bot" },
                Conversation = new ConversationAccount { Id = conversationMessage.ConversationId },
                ServiceUrl = conversationMessage.ServiceUrl,
                ChannelId = conversationMessage.ChannelId
            };

            await ((CloudAdapter)_adapter).ContinueConversationAsync(conversationMessage.MicrosoftAppId, conversation,
                async (context, token) =>
                {
                    var activity = MessageFactory.Text(conversationMessage.Message ?? string.Empty);
                    activity.TextFormat = TextFormatTypes.Markdown;

                    if (conversationMessage.Images is not null && conversationMessage.Images.Count > 0)
                    {
                        var attachments = conversationMessage.Images.Select(img => new Attachment
                        {
                            ContentType = "image",
                            ContentUrl = img
                        }).ToList();
                        activity.Attachments = attachments;
                    }
                    await context.SendActivityAsync(activity, cancellationToken: token);
                },
                CancellationToken.None);

            sendMessageResult.SentTime = DateTimeOffset.UtcNow.DateTime;
        }
        catch (Exception exception)
        {
            await WriteLogAsync(conversationMessage, exception);
            sendMessageResult = new SendMessageResult
            {
                MessageId = conversationMessage.MessageId,
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
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
            var url = new Uri($"{_appSettings.ForwardUrl}");
            var response = await _httpClient.PostAsJsonAsync(url, conversationMessage);
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
            _fileLogger.LogWarning(logMessage.ToString());

            return new SendMessageResult
            {
                MessageId = conversationMessage.MessageId,
                ErrorCode = 1,
                ErrorMessage = exception.Message,
                ConversationId = conversationMessage.ConversationId,
                SentTime = DateTimeOffset.UtcNow.DateTime
            };
        }
    }

    private async Task<ChatbotToken> GetTokenAsync()
    {
        var url = new Uri($"{_appSettings.AuthenticateUrl}");
        var authData = new Dictionary<string, string>
        {
            { "username", _appSettings.AppCredentialId },
            { "password", _appSettings.AppCredentialPassword }
        };
        using var authDataContent = new FormUrlEncodedContent(authData);
        var response = await _httpClient.PostAsync(url, authDataContent);
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

        await _logger.ErrorAsync(logMessage.ToString());
    }
}