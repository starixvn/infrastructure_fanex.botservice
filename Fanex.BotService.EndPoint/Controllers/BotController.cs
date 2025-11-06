namespace Fanex.BotService.EndPoint.Controllers;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

[Route("api/messages")]
public class BotController(IBotFrameworkHttpAdapter adapter, IBot bot) : ControllerBase
{
    [HttpPost]
    public async Task PostAsync()
    {
        await adapter.ProcessAsync(Request, Response, bot);
    }
}