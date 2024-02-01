namespace Fanex.NCB.BotService.EndPoint.Tests
{
    using System.Net;
    using AutoFixture;
    using Fanex.NCB.BotService.EndPoint.Controllers;
    using Fanex.NCB.Shared;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Xunit;

    public class WhoamiControllerTests
    {
        [Fact]
        public void Index_WhenCalled_ReturnExpectedContent()
        {
            var fixture = new Fixture();
            var controller = new WhoamiController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
            controller.ControllerContext.HttpContext.Connection.LocalIpAddress = new IPAddress(fixture.Create<long>());
            controller.ControllerContext.HttpContext.Connection.RemoteIpAddress = new IPAddress(fixture.Create<long>());
            controller.ControllerContext.HttpContext.Request.Scheme = fixture.Create<string>();
            controller.ControllerContext.HttpContext.Request.Path = $"/{fixture.Create<string>()}";
            controller.ControllerContext.HttpContext.Request.Headers["device-id"] = "20317";

            var expected = WhoamiHelper.GetContent(controller.ControllerContext.HttpContext);
            var actual = controller.Whoami();
            var viewResult = Assert.IsType<ContentResult>(actual);
            Assert.Equal(expected, viewResult.Content);
        }
    }
}