namespace Fanex.BotService.EndPoint.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : ControllerBase
    {
        public IActionResult Index()
        {
            return Content("Fanex bot is ready!");
        }
    }
}