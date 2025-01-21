using Fanex.BotService.Service.Core.Enum;
using Fanex.BotService.Service.Core.Services;
using Fanex.BotService.Shared.Models.Message;
using Fanex.BotService.Shared.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;

namespace Fanex.BotService.Core.Tests.Services;

public class BotClientServiceTests
{
    private readonly BotClientService botClientService;
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ICommonLogService commonLogService;

    public BotClientServiceTests()
    {
        httpClientFactory = Substitute.For<IHttpClientFactory>();
        var memoryCache = Substitute.For<IMemoryCache>();
        commonLogService = Substitute.For<ICommonLogService>();
        botClientService = new BotClientService(httpClientFactory, memoryCache, commonLogService);
    }

    [Fact]
    public async Task SendMessageAsync_ShouldReturnSendMessageResult()
    {
        // Arrange
        var botChannel = BotChannel.Skype;
        var message = new MessageEndpoint { MessageId = 1, ConversationId = "123" };

        // Act
        var result = await botClientService.SendMessageAsync(botChannel, message);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(message.MessageId, result.MessageId);
        Assert.Equal(message.ConversationId, result.ConversationId);
    }

    [Fact]
    public async Task SendMessageAsync_ShouldLogError_WhenExceptionThrown()
    {
        // Arrange
        var botChannel = BotChannel.Skype;
        var message = new MessageEndpoint { MessageId = 1, ConversationId = "123" };
        httpClientFactory.CreateClient(Arg.Any<string>()).Returns(x => { throw new HttpRequestException(); });

        // Act
        var result = await botClientService.SendMessageAsync(botChannel, message);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.ErrorCode);
        await commonLogService.Received(1).SentryErrorAsync(Arg.Any<string>(), Arg.Any<Exception>());
    }
}