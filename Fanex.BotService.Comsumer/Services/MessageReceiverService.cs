namespace Fanex.BotService.Comsumer.Services
{
    using System.Threading.Tasks;
    using Fanex.BotService.Comsumer.Services.Interfaces;
    using Fanex.BotService.Service.Core.Constants;
    using Fanex.BotService.Service.Core.Criteria.Message;
    using Fanex.BotService.Service.Core.Models;
    using Fanex.BotService.Service.Core.Services.Interfaces;
    using Fanex.BotService.Shared.Models.Message;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;

    public class MessageReceiverService : IMessageReceiverService
    {
        private readonly IBaseService baseService;
        private readonly IBotClientService botClientService;
        private readonly IMessageService messageService;

        public MessageReceiverService(IBaseService baseService, IBotClientService botClientService, IMessageService messageService)
        {
            this.baseService = baseService;
            this.botClientService = botClientService;
            this.messageService = messageService;
        }

        public async Task ConversationConsumerHandlerAsync(Conversation conversation)
        {
            int messageId = await messageService.InsertNewMessageAsync(conversation);
            Message message = GenerateMessage(messageId, conversation);
            SendMessageResult result = await botClientService.SendMessageAsync(conversation.Channel, message);
            await UpdateMessageStatusResult(result);
        }

        private async Task UpdateMessageStatusResult(SendMessageResult result)
        {
            var command = new UpdateMessageStatusCommand
            {
                MessageId = result.MessageId,
                Status = result.IsSucessfull ? MessageStatuses.Success : MessageStatuses.Failed
            };
            await baseService.ExecuteNonQueryAsync(command);
        }

        private static Message GenerateMessage(int messageId, Conversation conversation)
           => new()
           {
               MessageId = messageId,
               AppId = conversation.AppId,
               ChannelId = (byte)conversation.Channel,
               ConversationId = conversation.ConversationId,
               Content = conversation.Content,
           };
    }
}