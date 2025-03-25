using Fanex.BotService.Core.Models;
using Fanex.BotService.Core.Services;
using Fanex.BotService.Core.Services.Interfaces;
using Fanex.BotService.Shared.Services.Interfaces;
using Fanex.Data.Repository;
using Microsoft.Extensions.Caching.Memory;
using NSubstitute;

namespace Fanex.BotService.Endpoint.Core.Tests.Services;

public class ApplicationSettingServiceTests
{
    private readonly IBaseService _baseService;
    private readonly IMemoryCache _memoryCache;
    private readonly IApplicationSettingService _applicationSettingService;

    public ApplicationSettingServiceTests()
    {
        _baseService = Substitute.For<IBaseService>();
        _memoryCache = Substitute.For<IMemoryCache>();
        _applicationSettingService = new ApplicationSettingService(_baseService, _memoryCache);
    }

    [Fact]
    public async Task GetApplicationSettingAsync_ShouldReturnSettingsFromCache_WhenCacheIsNotEmpty()
    {
        // Arrange
        var cachedSettings = new List<ApplicationSetting>
        {
            new ApplicationSetting { AppId = 1, MicrosoftAppPassword = "encryptedPassword" }
        };
        _memoryCache.TryGetValue(Arg.Any<string>(), out Arg.Any<object>()!).Returns(x =>
        {
            x[1] = cachedSettings;
            return true;
        });

        // Act
        var result = await _applicationSettingService.GetApplicationSettingAsync();

        // Assert
        Assert.Equal(cachedSettings, result);
    }

    [Fact]
    public async Task GetApplicationSettingAsync_ShouldReturnSettingsFromService_WhenCacheIsEmpty()
    {
        // Arrange
        var serviceSettings = new List<ApplicationSetting>
        {
            new() { AppId = 1, MicrosoftAppPassword = "VtQK3Joi9l1PFXjctAqNcg==" }
        };
        _memoryCache.TryGetValue(Arg.Any<string>(), out Arg.Any<object>()!).Returns(false);
        _baseService.GetDataAsync<ApplicationSetting>(Arg.Any<ICriteria>()).Returns(serviceSettings);

        // Act
        var result = await _applicationSettingService.GetApplicationSettingAsync();

        // Assert
        Assert.Equal(serviceSettings, result);
        await _baseService.Received(1).GetDataAsync<ApplicationSetting>(Arg.Any<ICriteria>());
    }

    [Fact]
    public async Task GetApplicationSettingAsync_ShouldDecryptPasswords_WhenSettingsAreFetchedFromService()
    {
        // Arrange
        var serviceSettings = new List<ApplicationSetting>
        {
            new() { AppId = 1, MicrosoftAppPassword = "VtQK3Joi9l1PFXjctAqNcg==" }
        };
        _memoryCache.TryGetValue(Arg.Any<string>(), out Arg.Any<object>()!).Returns(false);
        _baseService.GetDataAsync<ApplicationSetting>(Arg.Any<ICriteria>()).Returns(serviceSettings);

        // Act
        var result = await _applicationSettingService.GetApplicationSettingAsync();

        // Assert
        Assert.All(result, setting => Assert.NotEqual("VtQK3Joi9l1PFXjctAqNcg==", setting.MicrosoftAppPassword));
    }
}