namespace Fanex.BotService.Api.Controllers
{
    using Fanex.BotService.Shared.Services.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly ICommonLogService logService;

        public HomeController(ICommonLogService logService)
        {
            this.logService = logService;
        }

        public IActionResult Index()
        {
            return Ok("Bot service API had ready!!!");
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("test-sentry")]
        public async Task<ObjectResult> Test()
        {
            await logService.SentryErrorAsync("Oliver test sentry", new Exception("Bug"));

            return Ok("ok");
        }
    }
}