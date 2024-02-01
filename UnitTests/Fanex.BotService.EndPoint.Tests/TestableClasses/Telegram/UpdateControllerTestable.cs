namespace Fanex.NCB.BotService.EndPoint.Tests.TestableClasses.Telegram
{
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Fanex.NCB.BotService.Core.Services;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.BotService.EndPoint.Controllers.Telegram;
    using Fanex.NCB.Shared.Models;
    using global::Telegram.Bot.Types;

    public class UpdateControllerTestable : UpdateController
    {
        public UpdateControllerTestable(
            ILogger logger,
            ITelegramBotService botService,
            IConversationService conversationService) : base(logger, botService, conversationService)
        {
        }

        public async Task<CommandResult> Public_EditChatTitledAsync(Message message)
        {
            return await base.EditChatTitledAsync(message);
        }

        public async Task<CommandResult> Public_AddBotToGroupAsync(Message message, User bot)
        {
            return await base.AddBotToGroupAsync(message, bot);
        }

        public async Task<CommandResult> Public_RemoveBotToGroupAsync(Message message)
        {
            return await base.RemoveBotToGroupAsync(message);
        }

        public async Task<CommandResult> Public_MigratedToSupergroupAsync(Message message)
        {
            return await base.MigratedToSupergroupAsync(message);
        }
    }
}