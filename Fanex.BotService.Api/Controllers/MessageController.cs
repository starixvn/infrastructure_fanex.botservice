namespace Fanex.BotService.Api.Controllers
{
    using Fanex.BotService.Service.Core.Enum;
    using Fanex.BotService.Service.Core.Models;
    using Fanex.BotService.Service.Core.Services.Interfaces;
    using Fanex.BotService.Shared.Models.Message;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class MessageController : Controller
    {
        private readonly IMessageQueueService queueService;

        public MessageController(IMessageQueueService queueService)
        {
            this.queueService = queueService;
        }

        [HttpPost]
        [Route("api/{channel}/send")]
        public async Task<IActionResult> SendMessageAsync(BotChannel channel, [FromBody] Conversation conversation)
        {
            if (!IsValidConversation(conversation))
            {
                return BadRequest();
            }
            conversation.Channel = channel;
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

        private static bool IsValidConversation(Conversation conversation)
            => conversation is not null &&
                !string.IsNullOrEmpty(conversation.ConversationId) &&
                !string.IsNullOrEmpty(conversation.Content) &&
                conversation.AppId > 0;

        private static bool IsValidMessageEndpoint(MessageEndpoint message)
            => message is not null &&
                message.MessageId > 0 &&
                !string.IsNullOrEmpty(message.ConversationId) &&
                !string.IsNullOrEmpty(message.Content) &&
                message.AppId > 0;
    }
}