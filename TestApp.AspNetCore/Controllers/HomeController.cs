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
            //await botService.SendSkypeAsync("29:1otxSNyz7rewcKJyycITsF1CEphLdZLvT5wabXgoePqQ", $"Msg: Hi Oliver, this is the message Test Skype 1");
            string content = "REFUND Settled Tickets MatchID: 83525304 Event Date: 2024/04/11 02:15 Sport Name: Virtual Football Cup Total tickets: 72 Affected Credit sites: 789y, NV88 Affected Licensee sites: 9wickets Attn: Due to the wrong calculation, \"VL Lisbon -vs- VL Amsterdam\" bet ID: \"\" has been re-settled. Kindly double check your statement again. Sorry for the inconvenience caused! 请注意:<虚拟运动> 由于技术有误, \"虚拟 里斯本 -vs- 虚拟 阿姆斯特丹\" 注单: \"\" 投注金额已被退还.不便之处 敬请见谅!";
            await botService.SendTelegramAsync("1064787034", content.Replace("[", "<").Replace("]", ">"));

            return Ok("OK");
        }
    }
}