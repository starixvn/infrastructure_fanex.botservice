using Fanex.BotService.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.BotService.Consumer.Controllers
{
    public class HomeController(ICommonLogService logService) : Controller
    {
        public IActionResult Index()
        {
            return Ok("Consumer running ...");
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("test-sentry")]
        public async Task<ObjectResult> Test()
        {
            await logService.SentryErrorAsync("test sentry");

            return Ok("ok");
        }
    }
}