using Fanex.BotService.Api.Controllers;
using Fanex.BotService.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Fanex.BotService.Api.Tests.Controllers;

public class HomeControllerTests
{
    private readonly ICommonLogService logService;
    private readonly HomeController homeController;

    public HomeControllerTests()
    {
        logService = Substitute.For<ICommonLogService>();
        homeController = new HomeController(logService);
    }

    [Fact]
    public void Index_ShouldReturnOkResult()
    {
        // Act
        var result = homeController.Index();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("Bot service API had ready!!!", okResult.Value);
    }

    [Fact]
    public async Task Test_ShouldReturnOkResult()
    {
        // Arrange
        logService.SentryErrorAsync(Arg.Any<string>(), Arg.Any<Exception>()).Returns(Task.CompletedTask);

        // Act
        var result = await homeController.Test();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal("ok", okResult.Value);
        await logService.Received(1).SentryErrorAsync("Oliver test sentry", Arg.Any<Exception>());
    }
}