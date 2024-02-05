namespace Fanex.BotService.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return Ok("Bot service API had ready!!!");
        }
    }
}