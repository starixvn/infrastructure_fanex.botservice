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
                $"Msg: Hi Oliver, this is the message Test Skype 3",
                [
                    "https://licbot.nexdev.net/crmupload/road-to-the-bluebonnet-house.jpg"
                ]);
            }
            catch (Exception ex)
            {
            }

            return Ok("OK");
        }
    }
}