namespace Fanex.BotService.Shared.Models.Message
{
    public class MessageEndpoint
    {
        public int MessageId { get; set; }

        public int AppId { get; set; }

        public string ConversationId { get; set; }

        public string Content { get; set; }

        public byte ChannelId { get; set; }
    }
}