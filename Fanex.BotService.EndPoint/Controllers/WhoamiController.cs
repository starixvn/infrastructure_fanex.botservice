namespace Fanex.BotService.EndPoint.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Shared;

    public class WhoamiController : Controller
    {
        [Route("Whoami")]
        [AllowAnonymous]
        public IActionResult WhoAmI()
        {
            return Content(WhoamiHelper.GetContent(HttpContext));
        }
    }
}