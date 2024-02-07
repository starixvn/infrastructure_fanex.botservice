using Fanex.BotService.Shared.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.BotService.Comsumer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICommonLogService logService;

        public HomeController(ICommonLogService logService)
        {
            this.logService = logService;
        }

        public IActionResult Index()
        {
            return Ok("Comsumer running ...");
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