namespace TestApp.AspNetCore.Controllers
{
    using Fanex.BotService.Client.Core.Enum;
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
            var conversationId = "a:1JHfHzZaosY0c2ZFBKV6lADDCgYV9JZnW2_iQy8y6AdEUJnIqn2H-lSK7xz4BYGLI7WVPQb3lE3qM-zGtRlt8tfiyLAzFQyC8bqEBSSFXLdvG2c3pZK0LZvBTIO8W2jy6";

            await botService.SendAsync(BotChannel.Teams,
            conversationId,
            $"Msg: Hi Oliver, this is the message Test Skype 5");

            return Ok("OK");
        }
    }
}