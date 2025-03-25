namespace TestApp.AspNetCore.Controllers
{
    using Fanex.BotService.Client.Core.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController(IBotService botService) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var conversationId = "29:1otxSNyz7rewcKJyycITsF1CEphLdZLvT5wabXgoePqQ";

            await botService.SendSkypeAsync(
            conversationId,
            $"Msg: Hi Oliver, this is the message Test Skype 5");

            return Ok("OK");
        }
    }
}