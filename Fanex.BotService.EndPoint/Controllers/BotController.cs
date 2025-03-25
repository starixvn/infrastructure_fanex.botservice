namespace Fanex.BotService.EndPoint.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using System.Threading.Tasks;

    [Route("api/messages")]
    public class BotController(IBotFrameworkHttpAdapter adapter, IBot bot) : Controller
    {
        [HttpPost]
        public async Task PostAsync()
        {
            await adapter.ProcessAsync(Request, Response, bot);
        }
    }
}