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
            var conversationId = "29:1otxSNyz7rewcKJyycITsF1CEphLdZLvT5wabXgoePqQ";
            try
            {
                await botService.SendMessageWithImagesAsync(BotChannel.Skype,
                conversationId,
                $"Msg: Hi Oliver, this is the message Test Skype 1",
                [
                    "https://t3.ftcdn.net/jpg/05/85/86/44/360_F_585864419_kgIYUcDQ0yiLOCo1aRjeu7kRxndcoitz.jpg"
                ]);
            }
            catch (Exception ex)
            {
            }

            return Ok("OK");
        }
    }
}