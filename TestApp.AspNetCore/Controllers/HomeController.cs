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
            await messengerService.SendSkypeAsync("29:1otxSNyz7rewcKJyycITsF1CEphLdZLvT5wabXgoePqQ", $"Msg: Hi Oliver, this is the message Test Skype");
            await messengerService.SendTelegramAsync("1064787034", $"Msg: Hi Oliver, this is the message Test Telegram");
            //for (int i = 0; i < 10; i++)
            //{
            //    await Task.WhenAll(messengerService.SendSkypeAsync("29:1otxSNyz7rewcKJyycITsF1CEphLdZLvT5wabXgoePqQ", $"Msg {i}: Hi Oliver, this is the message Test Skype"),
            //     messengerService.SendTelegramAsync("1064787034", $"Msg {i}: Hi Oliver, this is the message Test Telegram"));
            //    await Task.Delay(100);
            //}

            return Ok("OK");
        }
    }
}