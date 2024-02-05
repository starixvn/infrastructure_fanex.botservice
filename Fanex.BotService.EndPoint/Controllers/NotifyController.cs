namespace Fanex.NCB.BotService.EndPoint.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.BotService.Resources;
    using Fanex.BotService.Shared.Models.Message;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.NCB.BotService.Core.Criteria;
    using Fanex.NCB.BotService.Core.Enum;
    using Fanex.NCB.BotService.Core.Models;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotifyController : ControllerBase
    {
        private readonly IConversationService conversationService;
        private readonly IMessageValidationService messageLogService;
        private readonly ILogger logger;
        private readonly IMessageSenderService messageSenderService;

        public NotifyController(
            ILogger logger,
            IMessageValidationService messageLogService,
            IConversationService conversationService,
            IMessageSenderService messageSenderService
            )
        {
            this.logger = logger;
            this.conversationService = conversationService;
            this.messageLogService = messageLogService;
            this.messageSenderService = messageSenderService;
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/Test")]
        public async Task<ObjectResult> Test()
        {
            await logger.ErrorAsync("test sentry");

            return Ok("ok");
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/bot/send")]
        public async Task<ObjectResult> SendMessage([FromBody] IList<Message> messages)
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
        [Route("api/skype/send")]
        public async Task<ObjectResult> SendViaSkype([FromBody] IList<Message> messages)
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
        [Route("api/telegram/send")]
        public async Task<ObjectResult> SendViaTelegram([FromBody] IList<Message> messages)
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

        [HttpPost]
        [Route("api/bot/forward")]
        public async Task<ObjectResult> ForwardMessage([FromBody] ConversationMessage conversationMessage)
        {
            var result = await messageSenderService.SendMessageAsync(conversationMessage);

            return Ok(result);
        }

        private async Task<SendMessageResult> SendSingleMessage(IEnumerable<Bot> bots, int chanelId, Message message)
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
                }
            }
            else
            {
                sendMessageResult = new SendMessageResult
                {
                    ErrorCode = 1,
                    ErrorMessage = Resources.BotNotExist,
                    ConversationId = message.ConversationId,
                    SentTime = DateTimeOffset.UtcNow.DateTime
                };
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
            await logger.ErrorAsync(logMessage);
        }
    }
}