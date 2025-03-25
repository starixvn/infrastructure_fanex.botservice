using Fanex.BotService.EndPoint.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using NSubstitute;

namespace Fanex.BotService.Endpoint.Tests.Controllers
{
    public class BotControllerTests
    {
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly IBot _bot;
        private readonly BotController _controller;

        public BotControllerTests()
        {
            _adapter = Substitute.For<IBotFrameworkHttpAdapter>();
            _bot = Substitute.For<IBot>();
            _controller = new BotController(_adapter, _bot);
        }

        [Fact]
        public async Task PostAsync_Should_Call_ProcessAsync()
        {
            // Arrange
            var httpContext = new DefaultHttpContext();
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };

            // Act
            await _controller.PostAsync();

            // Assert
            await _adapter.Received(1).ProcessAsync(httpContext.Request, httpContext.Response, _bot);
        }
    }
}