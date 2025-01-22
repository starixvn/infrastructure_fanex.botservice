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
    private readonly MessageSenderService messageSenderService;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly AppSettings appSettings;
    private readonly IOptions<ProxySettings> proxySettings;
    private readonly ILogger<MessageSenderService> fileLogger;
    private readonly Logging.ILogger logger;
    private readonly IBotFrameworkHttpAdapter adapter;

    public MessageSenderServiceTests()
    {
        httpClientFactory = Substitute.For<IHttpClientFactory>();
        appSettings = new AppSettings { IsOnlySendIndirect = false };
        proxySettings = Substitute.For<IOptions<ProxySettings>>();
        fileLogger = Substitute.For<ILogger<MessageSenderService>>();
        logger = Substitute.For<Logging.ILogger>();
        adapter = Substitute.For<IBotFrameworkHttpAdapter>();
        proxySettings.Value.Returns(new ProxySettings { IsUseProxy = false, TimeOut = 30 });
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
        httpClientFactory.CreateClient().Returns(httpClient);
        messageSenderService = new MessageSenderService(
            httpClientFactory,
            appSettings,
            proxySettings,
            fileLogger,
            logger,
            adapter);
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
            ServiceURL = "serviceUrl",
            ChannelId = "channelId",
            ConversationBotId = "botId",
            BotName = "botName",
            Message = "Hello"
        };

        // Act
        var result = await messageSenderService.SendMessageAsync(conversationMessage);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.MessageId);
        Assert.Equal(conversationMessage.ConversationId, result.ConversationId);
    }

    [Fact]
    public async Task SendMessageAsync_ShouldSendIndirectMessage_WhenIsOnlySendIndirectIsTrue()
    {
        // Arrange
        appSettings.IsOnlySendIndirect = true;
        var conversationMessage = new ConversationMessage
        {
            MessageId = 1,
            ConversationId = "conversationId",
            MicrosoftAppId = "appId",
            ServiceURL = "serviceUrl",
            ChannelId = "channelId",
            ConversationBotId = "botId",
            BotName = "botName",
            Message = "Hello"
        };

        // Act
        var result = await messageSenderService.SendMessageAsync(conversationMessage);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(conversationMessage.MessageId, result.MessageId);
        Assert.Equal(conversationMessage.ConversationId, result.ConversationId);
    }
}