using Fanex.BotService.EndPoint.Controllers;
using Fanex.BotService.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Fanex.BotService.Endpoint.Tests.Controllers
{
    public class WhoAmIControllerTests
    {
        [Fact]
        public void Index_WhenCalled_ReturnExpectedContent()
        {
            var controller = new WhoamiController();

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };
            controller.ControllerContext.HttpContext.Connection.LocalIpAddress = new IPAddress(1234);
            controller.ControllerContext.HttpContext.Connection.RemoteIpAddress = new IPAddress(1234);
            controller.ControllerContext.HttpContext.Request.Scheme = "test string";
            controller.ControllerContext.HttpContext.Request.Path = $"/test-string";
            controller.ControllerContext.HttpContext.Request.Headers["device-id"] = "20317";

            var expected = WhoamiHelper.GetContent(controller.ControllerContext.HttpContext);
            var actual = controller.WhoAmI();
            var viewResult = Assert.IsType<ContentResult>(actual);
            Assert.Equal(expected, viewResult.Content);
        }
    }
}