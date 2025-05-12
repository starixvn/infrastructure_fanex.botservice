using Fanex.BotService.Core;
using Fanex.BotService.Core.Services;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Fanex.BotService.Endpoint.Core.Tests.Services;

public class FakeConversationServiceTests
{
    private readonly ILogger<MessageSenderService> _fileLogger;
    private readonly FakeConversationService _service;

    public FakeConversationServiceTests()
    {
        IList<BotConfigs> botConfigs = new List<BotConfigs>
        {
            new()
            {
                AppId = 1,
                MicrosoftAppId = "appId1",
                MicrosoftAppPassword = "password1",
                MicrosoftAppTenantId = "tenantId1",
                Channels = new List<Channel>
                {
                    new()
                    {
                        ChannelId = 1,
                        ChannelName = "channel1",
                        ServiceUrl = "serviceUrl1",
                        BotId = "botId1",
                        BotName = "botName1"
                    }
                }
            }
        };
        _fileLogger = Substitute.For<ILogger<MessageSenderService>>();
        _service = new FakeConversationService(botConfigs, _fileLogger);
    }

    [Fact]
    public async Task GetApplicationSettingAsync_ShouldReturnApplicationSettings()
    {
        var result = await _service.GetApplicationSettingAsync();

        var applicationSettings = result as Fanex.BotService.Core.Models.ApplicationSetting[] ?? result.ToArray();
        Assert.Single(applicationSettings);
        var setting = applicationSettings.First();
        Assert.Equal(1, setting.AppId);
        Assert.Equal("appId1", setting.MicrosoftAppId);
        Assert.Equal("password1", setting.MicrosoftAppPassword);
        Assert.Equal("tenantId1", setting.MicrosoftAppTenantId);
    }

    [Fact]
    public async Task GetBotAsync_ShouldReturnBots()
    {
        var result = await _service.GetBotAsync();

        var collection = result as Fanex.BotService.Core.Models.Bot[] ?? result.ToArray();
        Assert.Single(collection);
        var bot = collection.First();
        Assert.Equal(1, bot.AppId);
        Assert.Equal("appId1", bot.MicrosoftAppId);
        Assert.Equal(1, bot.ChannelId);
        Assert.Equal("channel1", bot.ChannelName);
        Assert.Equal("serviceUrl1", bot.ServiceUrl);
        Assert.Equal("botId1", bot.ConversationId);
        Assert.Equal("botName1", bot.BotName);
    }

    [Fact]
    public async Task AddBotContactAsync_ShouldLogCommand()
    {
        var turnContext = Substitute.For<ITurnContext>();
        turnContext.Activity.Returns(new Activity
        {
            Recipient = new ChannelAccount { Id = "recipientId", Name = "recipientName" },
            Conversation = new ConversationAccount { Id = "conversationId" },
            ChannelId = "channelId",
            From = new ChannelAccount { Id = "fromId", Name = "fromName" },
            ServiceUrl = "serviceUrl"
        });
        turnContext.TurnState.Returns(new TurnContextStateCollection
        {
            { "key", new ConnectorClient(new MicrosoftAppCredentials("appId", "password")) },
            { "key1", new ConnectorClient(new MicrosoftAppCredentials("appId1", "password1")) },
        });

        var result = await _service.AddBotContactAsync(turnContext);

        _fileLogger.ReceivedWithAnyArgs(1);
        Assert.NotNull(result);
    }

    // Add more tests for other methods similarly...
}