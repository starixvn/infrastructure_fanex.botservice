namespace Fanex.BotService.Service.Core.Services
{
    using Fanex.BotService.Service.Core.Constants;
    using Fanex.BotService.Service.Core.Criteria.Message;
    using Fanex.BotService.Service.Core.Models;
    using Fanex.BotService.Service.Core.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;

    public class MessageService : IMessageService
    {
        private readonly IBaseService baseService;

        public MessageService(IBaseService baseService)
        {
            this.baseService = baseService;
        }

        public async Task<int> InsertNewMessageAsync(Conversation conversation)
        {
            var command = new InsertNewMessageCommand
            {
                AppId = conversation.AppId,
                ConversationId = conversation.ConversationId,
                Content = conversation.Content,
                ChannelId = (byte)conversation.Channel,
                CreatedDate = DateTime.UtcNow,
                Status = MessageStatuses.New
            };
            await baseService.ExecuteNonQueryAsync(command);
            return command.MessageId;
        }

        public async Task<CommandResult> UpdateMessageStatusAsync(long messageId, string status)
        {
            var command = new UpdateMessageStatusCommand
            {
                MessageId = messageId,
                Status = status
            };
            return await baseService.ExecuteNonQueryAsync(command);
        }
    }
}