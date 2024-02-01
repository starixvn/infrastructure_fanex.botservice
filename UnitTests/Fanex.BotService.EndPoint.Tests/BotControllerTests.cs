namespace Fanex.NCB.BotService.EndPoint.Tests
{
    using System.Threading.Tasks;
    using Fanex.NCB.BotService.EndPoint.Controllers;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using NSubstitute;
    using Xunit;

    public class BotControllerTests
    {
        private IBotFrameworkHttpAdapter adapter;
        private IBot bot;

        [Fact]
        public async Task PostAsync_WhenCalled_ExecuteAdapter()
        {
            var controller = GetController();
            //Act
            await controller.PostAsync();

            await adapter.Received().ProcessAsync(Arg.Any<HttpRequest>(), Arg.Any<HttpResponse>(), bot);
        }

        private BotController GetController()
        {
            adapter = Substitute.For<IBotFrameworkHttpAdapter>();
            bot = Substitute.For<IBot>();
            return new BotController(adapter, bot);
        }
    }
}