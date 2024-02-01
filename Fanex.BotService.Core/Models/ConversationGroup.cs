namespace Fanex.NCB.BotService.Core.Models
{
    public class ConversationGroup
    {
        public string ConversationId { get; set; }

        public string GroupName { get; set; }
        
        public string ChannelId { get; set; }

        public string ServiceUrl { get; set; }

        public string BotConversationId { get; set; }

        public string BotName { get; set; }
    }
}
