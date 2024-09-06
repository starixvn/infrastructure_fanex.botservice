namespace Fanex.BotService.Api.Controllers
{
    using Fanex.BotService.Service.Core.Enum;
    using Fanex.BotService.Service.Core.Models;
    using Fanex.BotService.Service.Core.Services.Interfaces;
    using Fanex.BotService.Shared.Models.Message;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class MessageController(IMessageQueueService queueService) : Controller
    {
        private const byte MaxAttachments = 3;

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
        [Route("api/msteams/send")]
        public async Task<IActionResult> SendViaTeamsAsync(Conversation conversation)
        {
            if (!IsValidConversation(conversation, out string errMsg))
            {
                return BadRequest(errMsg);
            }
            conversation.Channel = BotChannel.Teams;
            await queueService.PublishAsync(conversation);

            return Ok();
        }

        [HttpPost]
        [Route("api/bot/send")]
        public async Task<IActionResult> SendAsync([FromBody] Conversation conversation)
        {
            if (!IsValidConversation(conversation, out string errMsg))
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

        private static bool IsValidAttchments(List<string> attachments)
        => attachments != null || attachments.Count > MaxAttachments;

        private static bool IsValidConversation(Conversation conversation, out string errMsg)
        {
            errMsg = !IsValidAttchments(conversation.Images) ? $"Exceeds number of attachments, max is {MaxAttachments}" : "Invalid input data";

            return conversation is not null &&
                !string.IsNullOrEmpty(conversation.ConversationId) &&
                !string.IsNullOrEmpty(conversation.Content) &&
                conversation.AppId > 0 &&
                IsValidAttchments(conversation.Images);
        }

        private static bool IsValidMessageEndpoint(MessageEndpoint message)
            => message is not null &&
                message.MessageId > 0 &&
                !string.IsNullOrEmpty(message.ConversationId) &&
                !string.IsNullOrEmpty(message.Content) &&
                message.AppId > 0;
    }
}