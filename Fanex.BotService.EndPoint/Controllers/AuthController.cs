namespace Fanex.BotService.EndPoint.Controllers;

using Core;
using Core.Services.Interfaces;
using Fanex.BotService.Core.Models;
using Fanex.BotService.Shared.Models.Message;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

public class AuthController(AppSettings appSettings,
    IConversationService conversationService,
    IMessageSenderService messageSenderService) : ControllerBase
{
    private const string InvalidUsernameOrPasswordText = "Username or password is incorrect";

    [AllowAnonymous]
    [HttpPost]
    [Route("api/authenticate")]
    public ObjectResult Authenticate(string username, string password)
    {
        if (IsValidCredential(username, password))
        {
            DateTime? expires = DateTimeOffset.UtcNow.DateTime.AddMinutes(appSettings.JwtLifeTime);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new Claim(ClaimTypes.Name, username)
                ]),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var securityToken = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(securityToken);
            return Ok(new { AccessToken = token, Expires = expires });
        }

        return BadRequest(InvalidUsernameOrPasswordText);
    }

    private bool IsValidCredential(string username, string password)
    {
        return username.Equals(appSettings.AppCredentialId, StringComparison.InvariantCultureIgnoreCase)
               && password.Equals(appSettings.AppCredentialPassword, StringComparison.InvariantCultureIgnoreCase);
    }

    [AllowAnonymous]
    [Route("api/bot/test")]
    public async Task<ObjectResult> TestBot()
    {
        var content =
            "**RESULT AMENDMENT NOTIFICATION** Please REVIEW and SEND this notification of the following event: Match ID: 83744253 Event Date: Mar 27, 2025 Event Time (GMT+8): Mar 27, 2025 12:07 PM League: Asia Cup 2016 (in Bangladesh) - Over 18 Total Runs Match: Zaheer Khan -vs- Raza Fraz **Notification ID: 10426** **Status: NEW** **English** Attn:[Cricket] Due to \"Toss Winner\" wrong result displayed, the match between \"Zaheer Khan -vs- Raza Fraz\" [Asia Cup 2016 (in Bangladesh) - Over 18 Total Runs - 27/03] as a result bet ID: \"@@\" has been re-settled. The actual result is \"Raza Fraz\" instead of \"Zaheer Khan\". Kindly double check your statement again. Sorry for the inconvenience caused! **Traditional Chinese** 請注意:[板球] 由於 \"贏得拋硬幣盤\" 賽果顯示錯誤, \"扎赫爾-汗 -vs- Raza Fraz\" [2016 亞洲杯(在孟加拉國) - 第十八輪跑位總得分 - 27/03] 注單: \"@@\" 已重新結算. 正確之賽果為 \"Raza Fraz\" 而不是 \"扎赫爾-汗\". 請重新檢查您的帳目. 不便之處 敬請見諒! **Simplified Chinese** 请注意:[板球] 由于 \"赢得抛硬币盘\" 赛果显示错误, \"扎赫尔-汗 -vs- Raza Fraz CN\" [2016 亚洲杯(在孟加拉国) - 第十八轮跑位总得分 - 27/03] 注单: \"@@\" 已重新结算. 正确之赛果为 \"Raza Fraz CN\" 而不是 \"扎赫尔-汗\". 请重新检查您的帐目. 不便之处 敬请见谅! **New Simplified Chinese** 请注意:[板球] 由于 \"赢得抛硬币盘\" 赛果显示错误, \"扎赫尔-汗 -vs- Raza Fraz SC2\" [2016 亚洲杯(在孟加拉国) - 第十八轮跑位总得分 - 27/03] 注单: \"@@\" 已重新结算. 正确之赛果为 \"Raza Fraz SC2\" 而不是 \"扎赫尔-汗\". 请重新检查您的帐目. 不便之处 敬请见谅!";
        var message = new MessageEndpoint
        {
            AppId = 1018,
            ChannelId = 2,
            Content = BotHelper.ReplaceSpecialCharacterOnTelegramMessageContent(content),
            ConversationId = "1064787034",
            MessageId = 1
        };
        var bots = await conversationService.GetBotAsync();
        var bot = bots.FirstOrDefault(t => IsValidBot(t, message.ChannelId, message.AppId));
        var conversationMessage = new ConversationMessage
        {
            MicrosoftAppId = bot.MicrosoftAppId,
            ConversationId = message.ConversationId,
            Message = message.Content,
            ServiceUrl = bot.ServiceUrl,
            ChannelId = bot.ChannelName,
            ConversationBotId = bot.ConversationId,
            BotName = bot.BotName,
            MessageId = message.MessageId,
            Images = message.Images
        };
        var result = await messageSenderService.SendMessageAsync(conversationMessage);

        return Ok(result);
    }

    private static bool IsValidBot(Bot bot, int channelId, int appId)
        => bot.ChannelId == channelId && bot.AppId == appId;
}