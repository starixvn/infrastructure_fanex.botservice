using Fanex.BotService.Consumer.Controllers;
using Fanex.BotService.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Fanex.BotService.Consumer.Tests
{
    public class HomeControllerTests
    {
        private readonly HomeController homeController;
        private readonly ICommonLogService logService;

        public HomeControllerTests()
        {
            logService = Substitute.For<ICommonLogService>();
            homeController = new HomeController(logService);
        }

        [Fact]
        public void Index_ReturnsOkResult()
        {
            // Act
            var result = homeController.Index();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal("Consumer running ...", okResult?.Value);
        }

        [Fact]
        public async Task Test_ReturnsOkResult()
        {
            // Act
            var result = await homeController.Test();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal("ok", okResult?.Value);
            await logService.Received(1).SentryErrorAsync("test sentry");
        }
    }
}