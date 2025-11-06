namespace Fanex.BotService.Service.Core.Services;

using Constants;
using Criteria.Message;
using Fanex.BotService.Shared.Models;
using Fanex.BotService.Shared.Services.Interfaces;
using Interfaces;
using Models;
using Newtonsoft.Json;

public class MessageService(IBaseService baseService, ICommonLogService commonLogService, IMessageQueueService queueMessageService) : IMessageService
{
    private const int ReSchedulerTimer = 10;

    public async Task<int> InsertNewMessageAsync(Conversation conversation, int retryTimeSpan)
    {
        var command = new InsertNewMessageCommand
        {
            AppId = conversation.AppId,
            ConversationId = conversation.ConversationId,
            Content = conversation.Content,
            Attachments = conversation.Images is not null && conversation.Images.Count > 0 ? JsonConvert.SerializeObject(conversation.Images) : string.Empty,
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
            await commonLogService.SentryErrorAsync($"Data: {JsonConvert.SerializeObject(conversation)} \r\n. ErrorMsg: {ex.Message}", ex);
            await queueMessageService.SchedulerPublishAsync(conversation, ReSchedulerTimer);
        }
        return command.MessageId;
    }

    public async Task UpdateMessageStatusAsync(SendMessageResult result, int retryTimeSpan)
    {
        try
        {
            var command = new UpdateMessageStatusCommand
            {
                MessageId = result.MessageId,
                ErrorMessage = result.ErrorMessage,
                Status = result.IsSuccessful ? MessageStatuses.Success : MessageStatuses.Failed
            };
            var commandResult = await baseService.ExecuteNonQueryAsync(command);
            if (!commandResult.IsSuccessful)
            {
                await commonLogService.SentryErrorAsync(commandResult.Message, new Exception(commandResult.Message));
            }
        }
        catch (Exception exception)
        {
            await commonLogService.SentryErrorAsync($"Message ID: {result.MessageId}. ErrMsg: {exception.Message}", exception);
            await queueMessageService.SchedulerPublishAsync(result, ReSchedulerTimer);
        }
    }
}