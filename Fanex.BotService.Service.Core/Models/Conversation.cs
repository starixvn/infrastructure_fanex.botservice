namespace Fanex.BotService.Service.Core.Models
{
    using Fanex.BotService.Service.Core.Enum;

    public class Conversation
    {
        public int AppId { get; set; }
        public string ConversationId { get; set; }
        public string Content { get; set; }
        public BotChannel Channel { get; set; }
    }
}