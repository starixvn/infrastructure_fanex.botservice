namespace Fanex.BotService.EndPoint.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.BotService.Core.Criteria;
    using Fanex.BotService.Core.Enum;
    using Fanex.BotService.Core.Models;
    using Fanex.BotService.Core.Services.Interfaces;
    using Fanex.BotService.Resources;
    using Fanex.BotService.Shared.Models.Message;
    using Fanex.BotService.Shared.Services.Interfaces;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NCB.Shared.Models;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotifyController : ControllerBase
    {
        private readonly IConversationService conversationService;
        private readonly IMessageValidationService messageLogService;
        private readonly ICommonLogService commonLogService;
        private readonly IMessageSenderService messageSenderService;

        public NotifyController(
            ICommonLogService commonLogService,
            IMessageValidationService messageLogService,
            IConversationService conversationService,
            IMessageSenderService messageSenderService
            )
        {
            this.commonLogService = commonLogService;
            this.conversationService = conversationService;
            this.messageLogService = messageLogService;
            this.messageSenderService = messageSenderService;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/Test")]
        public async Task<ObjectResult> Test()
        {
            await commonLogService.SentryErrorAsync("test sentry");

            return Ok("ok");
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/bot/send")]
        public async Task<ObjectResult> SendMessage([FromBody] IList<MessageEndpoint> messages)
        {
            var results = new List<SendMessageResult>();
            var bots = await conversationService.GetBotAsync();
            foreach (var message in messages)
            {
                var result = await SendSingleMessage(bots, message.ChannelId, message);
                results.Add(result);
            }

            return Ok(results);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/skype/send-multịpe")]
        public async Task<ObjectResult> SendViaSkype([FromBody] IList<MessageEndpoint> messages)
        {
            var results = new List<SendMessageResult>();
            var bots = await conversationService.GetBotAsync();
            foreach (var message in messages)
            {
                var result = await SendSingleMessage(bots, (byte)ConversationChannel.Skype, message);
                results.Add(result);
            }

            return Ok(results);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/telegram/send-multịpe")]
        public async Task<ObjectResult> SendViaTelegram([FromBody] IList<MessageEndpoint> messages)
        {
            var results = new List<SendMessageResult>();
            var bots = await conversationService.GetBotAsync();
            foreach (var message in messages)
            {
                var result = await SendSingleMessage(bots, (byte)ConversationChannel.Telegram, message);
                results.Add(result);
            }

            return Ok(results);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/msteams/send")]
        public async Task<IActionResult> SendSingleViaTeams([FromBody] MessageEndpoint message)
        {
            commonLogService.LogFileMessage($"Teams message: {JsonConvert.SerializeObject(message)}");
            var bots = await conversationService.GetBotAsync();
            var result = await SendSingleMessage(bots, (byte)ConversationChannel.Teams, message);
            return Ok(result);
        }

        [HttpPost]
        [Route("api/skype/send")]
        public async Task<IActionResult> SendSingleViaSkype([FromBody] MessageEndpoint message)
        {
            commonLogService.LogFileMessage($"Skype message: {JsonConvert.SerializeObject(message)}");
            var bots = await conversationService.GetBotAsync();
            var result = await SendSingleMessage(bots, (byte)ConversationChannel.Skype, message);
            return Ok(result);
        }

        [HttpPost]
        [Route("api/telegram/send")]
        public async Task<IActionResult> SendSingleViaTelegram([FromBody] MessageEndpoint message)
        {
            commonLogService.LogFileMessage($"Telegram message: {JsonConvert.SerializeObject(message)}");

            var bots = await conversationService.GetBotAsync();
            var result = await SendSingleMessage(bots, (byte)ConversationChannel.Telegram, message);
            return Ok(result);
        }

        [HttpPost]
        [Route("api/bot/forward")]
        public async Task<ObjectResult> ForwardMessage([FromBody] ConversationMessage conversationMessage)
        {
            var result = await messageSenderService.SendMessageAsync(conversationMessage);

            return Ok(result);
        }

        private async Task<SendMessageResult> SendSingleMessage(IEnumerable<Bot> bots, int chanelId, MessageEndpoint message)
        {
            SendMessageResult sendMessageResult;
            var startTime = DateTimeOffset.UtcNow.DateTime;
            var bot = bots.FirstOrDefault(t => IsValidBot(t, chanelId, message.AppId));
            if (bot != null)
            {
                bool isDuplicated = await messageLogService.IsInvalidSendMessageToBotService(message.ConversationId, message.Content);
                if (isDuplicated)
                {
                    sendMessageResult = DuplicatedResult(message.MessageId, message.ConversationId);
                }
                else
                {
                    string guiid = Guid.NewGuid().ToString();
                    await messageLogService.InsertMessageLogIntoCache(message.ConversationId, message.Content, guiid);

                    var conversationMessage = new ConversationMessage
                    {
                        MicrosoftAppId = bot.MicrosoftAppId,
                        ConversationId = message.ConversationId,
                        Message = message.Content,
                        ServiceURL = bot.ServiceUrl,
                        ChannelId = bot.ChannelName,
                        ConversationBotId = bot.ConversationId,
                        BotName = bot.BotName,
                        MessageId = message.MessageId
                    };
                    sendMessageResult = await messageSenderService.SendMessageAsync(conversationMessage);

                    if (!sendMessageResult.IsSucessfull)
                    {
                        await messageLogService.RemoveMessageLogOnCache(guiid);
                    }

                    var command = new AddMessageLogCommand
                    {
                        MessageId = message.MessageId,
                        Content = message.Content,
                        ConversationBotId = bot?.ConversationId,
                        ConversationId = message.ConversationId,
                        StartSentTime = startTime,
                        EndSentTime = DateTimeOffset.UtcNow.DateTime,
                        ErrorMessage = sendMessageResult.ErrorMessage,
                        IsSuccessfull = sendMessageResult.IsSucessfull
                    };
                    await conversationService.AddMessageLog(command);
                }
            }
            else
            {
                sendMessageResult = new SendMessageResult
                {
                    MessageId = message.MessageId,
                    ErrorCode = 1,
                    ErrorMessage = Resources.BotNotExist,
                    ConversationId = message.ConversationId,
                    SentTime = DateTimeOffset.UtcNow.DateTime
                };
            }

            return sendMessageResult;
        }

        private static bool IsValidBot(Bot bot, int channelId, int appId)
        {
            return bot.ChannelId == channelId && bot.AppId == appId;
        }

        private static SendMessageResult DuplicatedResult(long notificationId, string conversationId)
            => new()
            {
                ConversationId = conversationId,
                SentTime = DateTimeOffset.UtcNow.DateTime,
                ErrorCode = -1,
                ErrorMessage = Resources.DuplicatedErrorMessage,
                MessageId = notificationId,
                IsIgnoreRetry = true
            };

        protected internal virtual async Task WriteLogAsync(Exception exception)
        {
            var logMessage = new LogMessage(exception.Message)
                .AddSection("Database Info", DatabaseInfoGetters.GetDbExceptionInfo(exception))
                .AddSection("Server Info", CommonInfoGetters.GetServerInfo())
                .AddSection("Exception Info", CommonInfoGetters.GetExceptionInfo(exception))
                .ToString();
            await commonLogService.SentryErrorAsync(logMessage, exception);
        }
    }
}