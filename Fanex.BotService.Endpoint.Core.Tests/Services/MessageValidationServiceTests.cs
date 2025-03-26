using Fanex.BotService.Core.Models;
using Fanex.BotService.Core.Services;
using Fanex.Caching;
using Fanex.Logging;
using NSubstitute;

namespace Fanex.BotService.Endpoint.Core.Tests.Services;

public class MessageValidationServiceTests
{
    private readonly MessageValidationService _service;
    private readonly CheckDuplicatedMessageLogSetting _settings;
    private readonly ICacheService _cacheService;
    private readonly ILogger _logger;

    public MessageValidationServiceTests()
    {
        _settings = new CheckDuplicatedMessageLogSetting
        {
            IsEnabled = true,
            TimeSpanInMinutes = 10,
            ExceptionConversatiionIDs = ["exceptionId"]
        };
        _cacheService = Substitute.For<ICacheService>();
        _logger = Substitute.For<ILogger>();
        _service = new MessageValidationService(_settings, _cacheService, _logger);
    }

    [Fact]
    public async Task IsInvalidSendMessageToBotService_ShouldReturnFalse_WhenSettingsIsDisabled()
    {
        // Arrange
        _settings.IsEnabled = false;

        // Act
        var result = await _service.IsInvalidSendMessageToBotService("conversationId", "message");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsInvalidSendMessageToBotService_ShouldReturnFalse_WhenConversationIdIsException()
    {
        // Act
        var result = await _service.IsInvalidSendMessageToBotService("exceptionId", "message");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsInvalidSendMessageToBotService_ShouldReturnTrue_WhenMessageIsDuplicated()
    {
        // Arrange
        _cacheService.GetAsync<string>("conversationId").Returns("key1,key2");
        _cacheService.GetAsync<string>("key1").Returns("message");

        // Act
        var result = await _service.IsInvalidSendMessageToBotService("conversationId", "message");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task InsertMessageLogIntoCache_ShouldInsertMessageLog()
    {
        // Arrange
        _cacheService.GetAsync<string>("conversationId").Returns("key1");

        // Act
        await _service.InsertMessageLogIntoCache("conversationId", "messageContent", "guid");

        // Assert
        await _cacheService.Received().SetAsync("conversationId", "key1,guid", Arg.Any<CacheItemOptions>());
        await _cacheService.Received().SetAsync("guid", "messageContent", Arg.Any<CacheItemOptions>());
    }

    [Fact]
    public async Task RemoveMessageLogOnCache_ShouldRemoveMessageLog()
    {
        // Act
        await _service.RemoveMessageLogOnCache("guid");

        // Assert
        await _cacheService.Received().RemoveAsync("guid");
    }
}