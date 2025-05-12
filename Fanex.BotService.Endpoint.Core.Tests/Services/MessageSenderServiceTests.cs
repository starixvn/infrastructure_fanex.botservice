using Fanex.BotService.Core;
using Fanex.BotService.Core.Models;
using Fanex.BotService.Core.Services;
using Fanex.BotService.Shared.Models;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NSubstitute;
using System.Net;

namespace Fanex.BotService.Endpoint.Core.Tests.Services;

public class MessageSenderServiceTests
{
    private readonly MessageSenderService _messageSenderService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AppSettings _appSettings;
    private readonly IOptions<ProxySettings> _proxySettings;
    private readonly ILogger<MessageSenderService> _fileLogger;
    private readonly Logging.ILogger _logger;
    private readonly IBotFrameworkHttpAdapter _adapter;

    public MessageSenderServiceTests()
    {
        _httpClientFactory = Substitute.For<IHttpClientFactory>();
        _appSettings = new AppSettings { IsOnlySendIndirect = false };
        _proxySettings = Substitute.For<IOptions<ProxySettings>>();
        _fileLogger = Substitute.For<ILogger<MessageSenderService>>();
        _logger = Substitute.For<Logging.ILogger>();
        _adapter = Substitute.For<IBotFrameworkHttpAdapter>();
        _proxySettings.Value.Returns(new ProxySettings { IsUseProxy = false, TimeOut = 30 });
        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage()
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(new SendMessageResult
            {
                MessageId = 1,
                ConversationId = "conversationId"
            }))
        });
        var httpClient = new HttpClient(httpMessageHandler);
        _httpClientFactory.CreateClient().Returns(httpClient);
        _messageSenderService = new MessageSenderService(
            _httpClientFactory,
            _appSettings,
            _proxySettings,
            _fileLogger,
            _logger,
            _adapter);
    }

    [Fact]
    public async Task SendMessageAsync_ShouldSendDirectMessage_WhenIsOnlySendIndirectIsFalse()
    {
        // Arrange
        var conversationMessage = new ConversationMessage
        {
            MessageId = 1,
            ConversationId = "conversationId",
            MicrosoftAppId = "appId",
            ServiceUrl = "serviceUrl",
            ChannelId = "channelId",
            ConversationBotId = "botId",
            BotName = "botName",
            Message = "Hello"
        };

        // Act
        var result = await _messageSenderService.SendMessageAsync(conversationMessage);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.MessageId);
        Assert.Equal(conversationMessage.ConversationId, result.ConversationId);
    }

    [Fact]
    public async Task SendMessageAsync_ShouldSendIndirectMessage_WhenIsOnlySendIndirectIsTrue()
    {
        // Arrange
        _appSettings.IsOnlySendIndirect = true;
        var conversationMessage = new ConversationMessage
        {
            MessageId = 1,
            ConversationId = "conversationId",
            MicrosoftAppId = "appId",
            ServiceUrl = "serviceUrl",
            ChannelId = "channelId",
            ConversationBotId = "botId",
            BotName = "botName",
            Message = "Hello"
        };

        // Act
        var result = await _messageSenderService.SendMessageAsync(conversationMessage);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(conversationMessage.MessageId, result.MessageId);
        Assert.Equal(conversationMessage.ConversationId, result.ConversationId);
    }
}