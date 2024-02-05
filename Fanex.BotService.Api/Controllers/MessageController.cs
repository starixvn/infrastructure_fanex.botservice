namespace Fanex.BotService.Api.Controllers
{
    using Fanex.BotService.Service.Core.Enum;
    using Fanex.BotService.Service.Core.Models;
    using Fanex.BotService.Service.Core.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

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

        private static bool IsValidConversation(Conversation conversation)
        {
            return conversation is not null &&
                !string.IsNullOrEmpty(conversation.ConversationId) &&
                !string.IsNullOrEmpty(conversation.Content) &&
                conversation.AppId > 0;
        }
    }
}