using Fanex.BotService.EndPoint.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using NSubstitute;

namespace Fanex.BotService.EndPoint.Tests.Controllers
{
    public class BotControllerTests
    {
        private readonly IBotFrameworkHttpAdapter adapter;
        private readonly IBot bot;
        private readonly BotController controller;

        public BotControllerTests()
        {
            adapter = Substitute.For<IBotFrameworkHttpAdapter>();
            bot = Substitute.For<IBot>();
            controller = new BotController(adapter, bot);
        }

        [Fact]
        public async Task PostAsync_Should_Call_ProcessAsync()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            await controller.PostAsync();

            // Assert
            await adapter.Received(1).ProcessAsync(httpContext.Request, httpContext.Response, bot);
        }
    }
}