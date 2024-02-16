namespace TestApp.AspNetCore.Controllers
{
    using Fanex.BotService.Client.Core.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : Controller
    {
        private readonly IBotService messengerService;

        public HomeController(IBotService messengerService)
        {
            this.messengerService = messengerService;
        }

        public async Task<IActionResult> Index()
        {
            await Task.WhenAll(messengerService.SendSkypeAsync("29:1otxSNyz7rewcKJyycITsF1CEphLdZLvT5wabXgoePqQ", "Hi Oliver, this is the message Test Skype"),
                 messengerService.SendTelegramAsync("1064787034", "Hi Oliver, this is the message Test Telegram"));
            return Ok();
        }
    }
}