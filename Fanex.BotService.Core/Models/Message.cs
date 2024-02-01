namespace Fanex.NCB.BotService.Core.Models
{
    public class Message
    {
        public int MessageId { get; set; }

        public int AppId { get; set; }

        public string ConversationId { get; set; }

        public string Content { get; set; }

        public byte ChannelId { get; set; }
    }
}