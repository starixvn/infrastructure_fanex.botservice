using Fanex.BotService.Core;
using Fanex.BotService.Core.Criteria;
using Fanex.BotService.Core.Models;
using Fanex.BotService.Core.Services;
using Fanex.NCB.Shared.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;

namespace Fanex.BotService.Endpoint.Core.Tests.Services;

public class ConversationServiceTests
{
    private readonly IBaseService baseService;
    private readonly AppSettings appSettings;
    private readonly IMemoryCache memoryCache;
    private readonly ConversationService conversationService;

    public ConversationServiceTests()
    {
        baseService = Substitute.For<IBaseService>();
        appSettings = new AppSettings { BotListCacheLifeTime = 10 };
        memoryCache = Substitute.For<IMemoryCache>();
        conversationService = new ConversationService(baseService, appSettings, memoryCache);
    }

    [Fact]
    public async Task GetApplicationSettingAsync_ShouldReturnApplicationSettings()
    {
        // Arrange
        var expectedSettings = new List<ApplicationSetting> { new ApplicationSetting() };
        baseService.GetDataAsync<ApplicationSetting>(Arg.Any<GetApplicationSettingCriteria>())
            .Returns(Task.FromResult((IEnumerable<ApplicationSetting>)expectedSettings));

        // Act
        var result = await conversationService.GetApplicationSettingAsync();

        // Assert
        Assert.Equal(expectedSettings, result);
    }

    [Fact]
    public async Task GetBotAsync_ShouldReturnBotsFromCache()
    {
        // Arrange
        var expectedBots = new List<Bot> { new Bot() };
        memoryCache.TryGetValue(Arg.Any<string>(), out Arg.Any<object>()!).Returns(x =>
        {
            x[1] = expectedBots;
            return true;
        });

        // Act
        var result = await conversationService.GetBotAsync();

        // Assert
        Assert.Equal(expectedBots, result);
    }

    [Fact]
    public async Task GetBotAsync_ShouldReturnBotsFromServiceAndCacheThem()
    {
        // Arrange
        var expectedBots = new List<Bot> { new Bot { AppId = 1, BotName = "abc" } };
        memoryCache.TryGetValue(Arg.Any<string>(), out Arg.Any<object>()!).Returns(false);
        baseService.GetDataAsync<Bot>(Arg.Any<GetAllBotCriteria>())
            .Returns(expectedBots);

        // Act
        var result = await conversationService.GetBotAsync();

        // Assert
        memoryCache.ReceivedWithAnyArgs(1);

        Assert.Equal(expectedBots, result);
    }

    // Additional tests for other methods can be added here
}