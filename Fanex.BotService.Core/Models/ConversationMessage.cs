namespace Fanex.BotService.Core.Models
{
    using System.Collections.Generic;

    public class ConversationMessage
    {
        public long MessageId { get; set; }
        public string MicrosoftAppId { get; set; }
        public string ConversationId { get; set; }
        public string Message { get; set; }
        public string ServiceUrl { get; set; }
        public string ChannelId { get; set; }
        public string ConversationBotId { get; set; }
        public string BotName { get; set; }
        public List<string> Images { get; set; }
    }
}