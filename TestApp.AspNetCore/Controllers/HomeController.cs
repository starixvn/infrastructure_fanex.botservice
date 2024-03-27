namespace TestApp.AspNetCore.Controllers
{
    using Fanex.BotService.Client.Core.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly IBotService botService;

        public HomeController(IBotService botService)
        {
            this.botService = botService;
        }

        public async Task<IActionResult> Index()
        {
            await botService.SendSkypeAsync("29:1otxSNyz7rewcKJyycITsF1CEphLdZLvT5wabXgoePqQ", $"Msg: Hi Oliver, this is the message Test Skype 1");
            await botService.SendTelegramAsync("1064787034", $"Msg: Hi Oliver, this is the message Test Telegram 1");

            return Ok("OK");
        }
    }
}