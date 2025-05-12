namespace TestApp.AspNetCore.Controllers;

using Fanex.BotService.Client.Core.Enum;
using Fanex.BotService.Client.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

public class HomeController(IBotService botService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var conversationId = "1064787034";

        var content = "**RESULT AMENDMENT NOTIFICATION** Please REVIEW and SEND this notification of the following event: Match ID: 83744253 Event Date: Mar 27, 2025 Event Time (GMT+8): Mar 27, 2025 12:07 PM League: Asia Cup 2016 (in Bangladesh) - Over 18 Total Runs Match: Zaheer Khan -vs- Raza Fraz **Notification ID: 10426** **Status: NEW** **English** Attn:[Cricket] Due to \"Toss Winner\" wrong result displayed, the match between \"Zaheer Khan -vs- Raza Fraz\" [Asia Cup 2016 (in Bangladesh) - Over 18 Total Runs - 27/03] as a result bet ID: \"@@\" has been re-settled. The actual result is \"Raza Fraz\" instead of \"Zaheer Khan\". Kindly double check your statement again. Sorry for the inconvenience caused! **Traditional Chinese** 請注意:[板球] 由於 \"贏得拋硬幣盤\" 賽果顯示錯誤, \"扎赫爾-汗 -vs- Raza Fraz\" [2016 亞洲杯(在孟加拉國) - 第十八輪跑位總得分 - 27/03] 注單: \"@@\" 已重新結算. 正確之賽果為 \"Raza Fraz\" 而不是 \"扎赫爾-汗\". 請重新檢查您的帳目. 不便之處 敬請見諒! **Simplified Chinese** 请注意:[板球] 由于 \"赢得抛硬币盘\" 赛果显错误, \"扎赫尔-汗 -vs- Raza Fraz CN\" [2016 亚洲杯(在孟加拉国) - 第十八轮跑位总得分 - 27/03] 注单: \"@@\" 已重新结算. 正确之赛果为 \"Raza Fraz CN\" 而不是 \"扎赫尔-汗\". 请重新检查您的帐目. 不便之处 敬请见谅! **New Simplified Chinese** 请注意:[板球] 由于 \"赢得抛硬币盘\" 赛果显示错误, \"扎赫尔-汗 -vs- Raza Fraz SC2\" [2016 亚洲杯(在孟加拉国) - 第十八轮跑位总得分 - 27/03] 注单: \"@@\" 已重新结算. 正确之赛果为 \"Raza Fraz SC2\" 而不是 \"扎赫尔-汗\". 请重新检查您的帐目. 不便之处 敬请见谅!";
        await botService.SendAsync(BotChannel.Telegram, conversationId, ReplaceSpecialCharacterOnTelegramMessageContent(content));

        return Ok("OK");
    }

    public static string ReplaceSpecialCharacterOnTelegramMessageContent(string messageContent)
        => messageContent.Replace("[", "\\[").Replace("]", "\\]").Replace("\\", string.Empty);
}