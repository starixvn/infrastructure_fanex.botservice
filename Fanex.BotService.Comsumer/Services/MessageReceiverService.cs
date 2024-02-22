namespace Fanex.BotService.Comsumer.Services
{
    using System.Threading.Tasks;
    using Fanex.BotService.Comsumer.Services.Interfaces;
    using Fanex.BotService.Service.Core.Constants;
    using Fanex.BotService.Service.Core.Criteria.Message;
    using Fanex.BotService.Service.Core.Models;
    using Fanex.BotService.Service.Core.Services.Interfaces;
    using Fanex.BotService.Shared.Models.Message;
    using Fanex.BotService.Shared.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Newtonsoft.Json;

    public class MessageReceiverService : IMessageReceiverService
    {
        private readonly IBaseService baseService;
        private readonly IBotClientService botClientService;
        private readonly IMessageService messageService;
        private readonly ICommonLogService commonLogService;
        private readonly IMessageQueueService messageQueueService;

        public MessageReceiverService(IBaseService baseService,
            IBotClientService botClientService,
            IMessageService messageService,
            ICommonLogService commonLogService,
            IMessageQueueService messageQueueService)
        {
            this.baseService = baseService;
            this.botClientService = botClientService;
            this.messageService = messageService;
            this.commonLogService = commonLogService;
            this.messageQueueService = messageQueueService;
        }

        public async Task ConversationConsumerHandlerAsync(Conversation conversation)
        {
            commonLogService.LogFileMessage($"Conversation: {JsonConvert.SerializeObject(conversation)}");
            int messageId = await messageService.InsertNewMessageAsync(conversation);
            commonLogService.LogFileMessage($"Inserted message with messageID: {messageId}");
            if (messageId > 0)
            {
                MessageEndpoint message = GenerateMessage(messageId, conversation);
                SendMessageResult result = await botClientService.SendMessageAsync(conversation.Channel, message);
                commonLogService.LogFileMessage($"Send result: {JsonConvert.SerializeObject(result)}");
                await UpdateMessageStatusResult(result);
            }
        }

        public async Task UpdateMessageStatusResult(SendMessageResult result)
        {
            try
            {
                var command = new UpdateMessageStatusCommand
                {
                    MessageId = result.MessageId,
                    ErrorMessage = result.ErrorMessage,
                    Status = result.IsSucessfull ? MessageStatuses.Success : MessageStatuses.Failed
                };
                await baseService.ExecuteNonQueryAsync(command);
            }
            catch (Exception exception)
            {
                await messageQueueService.PublishAsync(result);
                await commonLogService.SentryErrorAsync(exception.Message, exception);
            }
        }

        private static MessageEndpoint GenerateMessage(int messageId, Conversation conversation)
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