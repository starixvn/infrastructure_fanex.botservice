namespace TestApp.AspNetCore.Controllers
{
    using Fanex.BotService.Client.Core.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController(IBotService botService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var conversationId = "1064787034";

            await botService.SendTelegramAsync(conversationId, "Msg: Hi Oliver, this is the message Test");

            return Ok("OK");
        }
    }
}