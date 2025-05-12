namespace Fanex.BotService.EndPoint.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Shared.Services.Interfaces;
using System.Threading.Tasks;

[Route("api/messages")]
public class BotController(IBotFrameworkHttpAdapter adapter, IBot bot, ICommonLogService logService) : Controller
{
    [HttpPost]
    public async Task PostAsync()
    {
        logService.LogFileMessage("Received messages");
        await adapter.ProcessAsync(Request, Response, bot);
    }
}