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
            string content = "Good Day! This is 1on1 Customer Service. Kindly take note of the below message [请注意以下公告]: Attn:[Soccer] Due to bets erroneously accepted in goal danger play during the match between \"Manchester United -vs- Arsenal\" [TESTING (Betting Prohibited Area!!) - ESport SOCCER - 15 Mins Play - 12/4], bet ID: \" \" which affected are considered VOID. Parlay counted as one (1). Cash Out Cancelled. Kindly double check your statement again. Sorry for the inconvenience caused! 请注意:[足球] 由于注单在有进球的危险时段中误被接受, \"曼联 -vs- 阿森纳\" [TESTING (Betting Prohibited Area!!) - ESport SOCCER - 15 Mins Play - 12/4], 注单: \" \" 只作废受影响的注单. 串关以(1)计算. 实时兑现被取消. 请重新检查您的帐目. 不便之处 敬请见谅! >> Above match we had re-settled thus it might caused certain accounts showing negative balance. Sorry for the inconvenience caused. >> 以上赛事已重新结算，这将导致某些帐号出现负数。敬请见谅。 Thank you for your kind attention ~";
            await botService.SendTelegramAsync("1064787034", content);

            return Ok("OK");
        }
    }
}