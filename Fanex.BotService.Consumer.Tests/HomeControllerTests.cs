using Fanex.BotService.Consumer.Controllers;
using Fanex.BotService.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;

namespace Fanex.BotService.Consumer.Tests
{
    public class HomeControllerTests
    {
        private readonly HomeController _homeController;
        private readonly ICommonLogService _logService;

        public HomeControllerTests()
        {
            _logService = Substitute.For<ICommonLogService>();
            _homeController = new HomeController(_logService);
        }

        [Fact]
        public void Index_ReturnsOkResult()
        {
            // Act
            var result = _homeController.Index();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal("Consumer running ...", okResult?.Value);
        }

        [Fact]
        public async Task Test_ReturnsOkResult()
        {
            // Act
            var result = await _homeController.Test();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.Equal("ok", okResult?.Value);
            await _logService.Received(1).SentryErrorAsync("test sentry");
        }
    }
}