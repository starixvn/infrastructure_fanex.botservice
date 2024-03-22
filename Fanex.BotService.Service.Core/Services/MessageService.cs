namespace Fanex.BotService.Service.Core.Services
{
    using Fanex.BotService.Service.Core.Constants;
    using Fanex.BotService.Service.Core.Criteria.Message;
    using Fanex.BotService.Service.Core.Models;
    using Fanex.BotService.Service.Core.Services.Interfaces;
    using Fanex.BotService.Shared.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;

    public class MessageService : IMessageService
    {
        private readonly IBaseService baseService;
        private readonly ICommonLogService commonLogService;
        private readonly IMessageQueueService queueMessageService;

        public MessageService(IBaseService baseService, ICommonLogService commonLogService, IMessageQueueService queueMessageService)
        {
            this.baseService = baseService;
            this.commonLogService = commonLogService;
            this.queueMessageService = queueMessageService;
        }

        public async Task<int> InsertNewMessageAsync(Conversation conversation)
        {
            var command = new InsertNewMessageCommand
            {
                AppId = conversation.AppId,
                ConversationId = conversation.ConversationId,
                Content = conversation.Content,
                ChannelId = (byte)conversation.Channel,
                CreatedDate = DateTimeOffset.UtcNow.DateTime,
                Status = MessageStatuses.New
            };
            try
            {
                await baseService.ExecuteNonQueryAsync(command);
            }
            catch (Exception ex)
            {
                await commonLogService.SentryErrorAsync(ex.Message, ex);
                await queueMessageService.PublishAsync(conversation, 5);
            }
            return command.MessageId;
        }

        public async Task UpdateMessageStatusAsync(SendMessageResult result)
        {
            try
            {
                var command = new UpdateMessageStatusCommand
                {
                    MessageId = result.MessageId,
                    ErrorMessage = result.ErrorMessage,
                    Status = result.IsSucessfull ? MessageStatuses.Success : MessageStatuses.Failed
                };
                var commandResult = await baseService.ExecuteNonQueryAsync(command);
                if (!commandResult.IsSuccessful)
                {
                    await commonLogService.SentryErrorAsync(commandResult.Message);
                }
            }
            catch (Exception exception)
            {
                await queueMessageService.PublishAsync(result);
                await commonLogService.SentryErrorAsync($"Message ID: {result.MessageId}. ErrMsg: {exception.Message}", exception);
                await Task.Delay(60000);
            }
        }
    }
}