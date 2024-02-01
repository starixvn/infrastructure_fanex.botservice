namespace Fanex.NCB.BotService.EndPoint.Tests.TestableClasses.Telegram
{
    using System;
    using System.Threading.Tasks;
    using Fanex.NCB.BotService.Core;
    using Fanex.NCB.BotService.Core.Services;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.BotService.EndPoint.Controllers.Telegram;
    using Fanex.NCB.Shared.Models;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class MessageControllerTestable : MessageController
    {
        public MessageControllerTestable(
            IOptions<AppSettings> appSettings,
            ILogger<MessageController> filelogger,
            Logging.ILogger logger,
            IConversationService conversationService,
            ITelegramBotService botService,
            IMessageLogService messageLogService) : base(appSettings, filelogger, logger, conversationService, botService, messageLogService)
        {
        }

        public async Task<SendMessageResult> Public_SendMessageAsync(ConversationInfo conversationInfo)
        {
            return await base.SendMessageAsync(conversationInfo);
        }

        public async Task Public_AddMessageLogToDatabaseAsync(ConversationInfo conversationInfo, SendMessageResult result)
        {
            await base.AddMessageLogToDatabaseAsync(conversationInfo, result);
        }

        public async Task Public_WriteLogAsync(Exception exception)
        {
            await base.WriteLogAsync(exception);
        }
    }
}