using Fanex.BotService.Api.Controllers;
using Fanex.BotService.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Fanex.BotService.Api.Tests.Controllers;

public class HomeControllerTests
{
    private readonly ICommonLogService _logService;
    private readonly HomeController _homeController;

    public HomeControllerTests()
    {
        _logService = Substitute.For<ICommonLogService>();
        _homeController = new HomeController(_logService);
    }

    [Fact]
    public void Index_ShouldReturnOkResult()
    {
        // Act
        var result = _homeController.Index();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Bot service API had ready!!!", okResult.Value);
    }

    [Fact]
    public async Task Test_ShouldReturnOkResult()
    {
        // Arrange
        _logService.SentryErrorAsync(Arg.Any<string>(), Arg.Any<Exception>()).Returns(Task.CompletedTask);

        // Act
        var result = await _homeController.Test();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("ok", okResult.Value);
        await _logService.Received(1).SentryErrorAsync("Oliver test sentry", Arg.Any<Exception>());
    }
}