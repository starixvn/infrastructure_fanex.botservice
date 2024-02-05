namespace Fanex.BotService.Client.Core.Models
{
    using System;
    using Fanex.BotService.Client.Core.Enum;

    internal class Conversation
    {
        public int AppId { get; set; }
        public string ConversationId { get; set; }
        public string Content { get; set; }
        public BotChannel? Channel { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTime.UtcNow;
    }
}