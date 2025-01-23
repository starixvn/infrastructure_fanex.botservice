namespace Fanex.BotService.Api.Controllers
{
    using Fanex.BotService.Service.Core.Enum;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;
    using Service.Core.Models;
    using Service.Core.Services.Interfaces;
    using Shared.Models.Message;

    public class MessageController(IMessageQueueService queueService) : Controller
    {
        private const byte MaxAttachments = 20;

        [HttpPost]
        [Route("api/{channel}/send")]
        public async Task<IActionResult> SendMessageAsync(BotChannel channel, [FromBody] Conversation conversation)
        {
            if (!IsValidConversation(conversation, out string errMsg))
            {
                return BadRequest(errMsg);
            }
            conversation.Channel = channel;
            await queueService.PublishAsync(conversation);

            return Ok();
        }

        [HttpPost]
        [Route("api/bot/send")]
        public async Task<IActionResult> SendAsync([FromBody] Conversation conversation)
        {
            if (!IsValidConversation(conversation, out var errMsg))
            {
                return BadRequest(errMsg);
            }
            await queueService.PublishAsync(conversation);

            return Ok();
        }

        [HttpPost]
        [Route("api/retry")]
        public async Task<IActionResult> RetryMessageAsync([FromBody] MessageEndpoint message)
        {
            if (!IsValidMessageEndpoint(message))
            {
                return BadRequest($"Invalid message ID: {JsonConvert.SerializeObject(message)}");
            }
            await queueService.PublishAsync(message);
            return Ok();
        }

        public static bool IsValidAttachments(List<string> attachments)
        => attachments is not { Count: > MaxAttachments };

        public static bool IsValidConversation(Conversation conversation, out string errMsg)
        {
            errMsg = !IsValidAttachments(conversation.Images) ? $"Exceeds number of attachments, max is {MaxAttachments}" : "Invalid input data";
            return IsValidConversationModel(conversation) &&
                IsValidAttachments(conversation.Images);
        }

        public static bool IsValidConversationModel(Conversation conversation)
            => !string.IsNullOrEmpty(conversation.ConversationId) &&
                !string.IsNullOrEmpty(conversation.Content) &&
                conversation.AppId > 0;

        public static bool IsValidMessageEndpoint(MessageEndpoint message)
            => IsValidMsgIdAndAppId(message) &&
               !string.IsNullOrEmpty(message.ConversationId) &&
               !string.IsNullOrEmpty(message.Content);

        private static bool IsValidMsgIdAndAppId(MessageEndpoint message)
            => message is not null &&
               message.MessageId > 0 &&
               message.AppId > 0;
    }
}