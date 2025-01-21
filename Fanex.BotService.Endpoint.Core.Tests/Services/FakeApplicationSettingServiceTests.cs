using Fanex.BotService.Core;
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
            new BotConfigs
            {
                AppId = 1,
                MicrosoftAppType = "Type1",
                MicrosoftAppId = "AppId1",
                MicrosoftAppPassword = "Password1",
                MicrosoftAppTenantId = "TenantId1"
            },
            new BotConfigs
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
        Assert.Equal(2, result.Count());
        Assert.Equal("AppId1", result.First().MicrosoftAppId);
        Assert.Equal("AppId2", result.Last().MicrosoftAppId);
    }
}