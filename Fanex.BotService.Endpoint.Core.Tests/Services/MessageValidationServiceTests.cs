using Fanex.BotService.Core.Models;
using Fanex.BotService.Core.Services;
using Fanex.Caching;
using Fanex.Logging;
using NSubstitute;

namespace Fanex.BotService.Endpoint.Core.Tests.Services;

public class MessageValidationServiceTests
{
    private readonly MessageValidationService service;
    private readonly CheckDuplicatedMessageLogSetting settings;
    private readonly ICacheService cacheService;
    private readonly ILogger logger;

    public MessageValidationServiceTests()
    {
        settings = new CheckDuplicatedMessageLogSetting
        {
            IsEnabled = true,
            TimeSpanInMinutes = 10,
            ExceptionConversatiionIDs = new string[] { "exceptionId" }
        };
        cacheService = Substitute.For<ICacheService>();
        logger = Substitute.For<ILogger>();
        service = new MessageValidationService(settings, cacheService, logger);
    }

    [Fact]
    public async Task IsInvalidSendMessageToBotService_ShouldReturnFalse_WhenSettingsIsDisabled()
    {
        // Arrange
        settings.IsEnabled = false;

        // Act
        var result = await service.IsInvalidSendMessageToBotService("conversationId", "message");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsInvalidSendMessageToBotService_ShouldReturnFalse_WhenConversationIdIsException()
    {
        // Act
        var result = await service.IsInvalidSendMessageToBotService("exceptionId", "message");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsInvalidSendMessageToBotService_ShouldReturnTrue_WhenMessageIsDuplicated()
    {
        // Arrange
        cacheService.GetAsync<string>("conversationId").Returns("key1,key2");
        cacheService.GetAsync<string>("key1").Returns("message");

        // Act
        var result = await service.IsInvalidSendMessageToBotService("conversationId", "message");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task InsertMessageLogIntoCache_ShouldInsertMessageLog()
    {
        // Arrange
        cacheService.GetAsync<string>("conversationId").Returns("key1");

        // Act
        await service.InsertMessageLogIntoCache("conversationId", "messageContent", "guiid");

        // Assert
        await cacheService.Received().SetAsync("conversationId", "key1,guiid", Arg.Any<CacheItemOptions>());
        await cacheService.Received().SetAsync("guiid", "messageContent", Arg.Any<CacheItemOptions>());
    }

    [Fact]
    public async Task RemoveMessageLogOnCache_ShouldRemoveMessageLog()
    {
        // Act
        await service.RemoveMessageLogOnCache("guid");

        // Assert
        await cacheService.Received().RemoveAsync("guid");
    }
}