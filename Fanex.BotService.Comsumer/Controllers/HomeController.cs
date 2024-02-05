using Microsoft.AspNetCore.Mvc;

namespace Fanex.BotService.Comsumer.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Ok("Comsumer running ...");
        }
    }
}