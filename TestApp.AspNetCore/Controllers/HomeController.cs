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
            await messengerService.SendSkypeAsync("29:1TIy3asOA1m-3Qm2paoNam8KtLjA2JO5Fu5T6n-yScwY", "Hi Oliver, this is the message Test 2");
            return Ok();
        }
    }
}