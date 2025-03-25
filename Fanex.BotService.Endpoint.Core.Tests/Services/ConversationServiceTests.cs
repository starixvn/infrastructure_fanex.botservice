using Fanex.BotService.Core;
using Fanex.BotService.Core.Criteria;
using Fanex.BotService.Core.Models;
using Fanex.BotService.Core.Services;
using Fanex.BotService.Shared.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;

namespace Fanex.BotService.Endpoint.Core.Tests.Services;

public class ConversationServiceTests
{
    private readonly IBaseService _baseService;
    private readonly AppSettings _appSettings;
    private readonly IMemoryCache _memoryCache;
    private readonly ConversationService _conversationService;

    public ConversationServiceTests()
    {
        _baseService = Substitute.For<IBaseService>();
        _appSettings = new AppSettings { BotListCacheLifeTime = 10 };
        _memoryCache = Substitute.For<IMemoryCache>();
        _conversationService = new ConversationService(_baseService, _appSettings, _memoryCache);
    }

    [Fact]
    public async Task GetApplicationSettingAsync_ShouldReturnApplicationSettings()
    {
        // Arrange
        var expectedSettings = new List<ApplicationSetting> { new ApplicationSetting() };
        _baseService.GetDataAsync<ApplicationSetting>(Arg.Any<GetApplicationSettingCriteria>())
            .Returns(Task.FromResult((IEnumerable<ApplicationSetting>)expectedSettings));

        // Act
        var result = await _conversationService.GetApplicationSettingAsync();

        // Assert
        Assert.Equal(expectedSettings, result);
    }

    [Fact]
    public async Task GetBotAsync_ShouldReturnBotsFromCache()
    {
        // Arrange
        var expectedBots = new List<Bot> { new Bot() };
        _memoryCache.TryGetValue(Arg.Any<string>(), out Arg.Any<object>()!).Returns(x =>
        {
            x[1] = expectedBots;
            return true;
        });

        // Act
        var result = await _conversationService.GetBotAsync();

        // Assert
        Assert.Equal(expectedBots, result);
    }

    [Fact]
    public async Task GetBotAsync_ShouldReturnBotsFromServiceAndCacheThem()
    {
        // Arrange
        var expectedBots = new List<Bot> { new Bot { AppId = 1, BotName = "abc" } };
        _memoryCache.TryGetValue(Arg.Any<string>(), out Arg.Any<object>()!).Returns(false);
        _baseService.GetDataAsync<Bot>(Arg.Any<GetAllBotCriteria>())
            .Returns(expectedBots);

        // Act
        var result = await _conversationService.GetBotAsync();

        // Assert
        _memoryCache.ReceivedWithAnyArgs(1);

        Assert.Equal(expectedBots, result);
    }

    // Additional tests for other methods can be added here
}