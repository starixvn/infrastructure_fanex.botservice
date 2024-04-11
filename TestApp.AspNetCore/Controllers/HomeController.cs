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
            await botService.SendTelegramAsync("1064787034", $"Internal Department Request No.: N/A Event Start Time: 2:00 PM League: \\*\\*SABA Virtual EURO CUP April 2020 Group B \\(In Spain\\) \\- 20 Mins Play Match: Manchester United - vs - Arsenal **Affected Credit sites: 777** **Affected Licensee sites:** Bettype: Live 1H Over/Under Total: 2 Reason: Group Betting Results: HT N/A, FT N/A Decided by: N/A Requested by: N/A Voided by: starix19 Attn:[Soccer] Due to abnormal bets, the match between \"Manchester United -vs- Arsenal\" [**Virtual EURO CUP April 2020 Group B (In Spain) - 20 Mins Play - 11/4], bet ID: \" \" are considered VOID. Sorry for the inconvenience caused! 请注意:[足球] 由于非正常投注, \"曼联 -vs- 阿森纳\" [**虚拟欧洲杯4月2020 B组(在西班牙) - 20分钟赛事 - 11/4], 注单: \" \" 已被作废. 不便之处 敬请见谅!");

            return Ok("OK");
        }
    }
}