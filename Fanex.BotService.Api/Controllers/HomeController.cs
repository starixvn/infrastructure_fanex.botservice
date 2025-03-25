namespace Fanex.BotService.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Shared.Services.Interfaces;

    public class HomeController(ICommonLogService logService) : Controller
    {
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