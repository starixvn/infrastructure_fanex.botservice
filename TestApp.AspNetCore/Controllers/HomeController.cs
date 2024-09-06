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
            await botService.SendMessageWithImagesAsync(BotChannel.Skype,
                "29:1otxSNyz7rewcKJyycITsF1CEphLdZLvT5wabXgoePqQ",
                $"Msg: Hi Oliver, this is the message Test Skype 1",
                [
                    "https://t3.ftcdn.net/jpg/05/85/86/44/360_F_585864419_kgIYUcDQ0yiLOCo1aRjeu7kRxndcoitz.jpg",
                    "https://img.freepik.com/free-photo/snowy-mountain-peak-starry-galaxy-majesty-generative-ai_188544-9650.jpg",
                    "https://i.pinimg.com/564x/4b/05/0c/4b050ca4fcf588eedc58aa6135f5eecf.jpg"
                ]);

            return Ok("OK");
        }
    }
}