using Fanex.BotService.Core;
using Fanex.BotService.Core.Models;
using Fanex.BotService.Core.Services;
using Fanex.BotService.Core.Services.Interfaces;

namespace Fanex.BotService.Endpoint.Core.Tests.Services;

public class FakeApplicationSettingServiceTests
{
    private readonly IApplicationSettingService _service;
    private readonly IList<BotConfigs> _botConfigs;

    public FakeApplicationSettingServiceTests()
    {
        _botConfigs = new List<BotConfigs>
        {
            new()
            {
                AppId = 1,
                MicrosoftAppType = "Type1",
                MicrosoftAppId = "AppId1",
                MicrosoftAppPassword = "Password1",
                MicrosoftAppTenantId = "TenantId1"
            },
            new()
            {
                AppId = 2,
                MicrosoftAppType = "Type2",
                MicrosoftAppId = "AppId2",
                MicrosoftAppPassword = "Password2",
                MicrosoftAppTenantId = "TenantId2"
            }
        };

        _service = new FakeApplicationSettingService(_botConfigs);
    }

    [Fact]
    public async Task GetApplicationSettingAsync_ShouldReturnApplicationSettings()
    {
        // Act
        var result = await _service.GetApplicationSettingAsync();

        // Assert
        Assert.NotNull(result);
        var applicationSettings = result as ApplicationSetting[] ?? result.ToArray();
        Assert.Equal(2, applicationSettings.Length);
        Assert.Equal("AppId1", applicationSettings.First().MicrosoftAppId);
        Assert.Equal("AppId2", applicationSettings.Last().MicrosoftAppId);
    }
}