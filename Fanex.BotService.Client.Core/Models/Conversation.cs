namespace Fanex.BotService.Client.Core.Models
{
    using System;
    using System.Collections.Generic;
    using Fanex.BotService.Client.Core.Enum;

    internal class Conversation
    {
        public int AppId { get; set; }
        public string ConversationId { get; set; }
        public string Content { get; set; }
        public BotChannel? Channel { get; set; }
        public List<string> Images { get; set; }
        public DateTime? CreatedDate { get; set; } = DateTimeOffset.UtcNow.DateTime;
    }
}