namespace Fanex.BotService.Service.Core.Criteria.Message
{
    using Fanex.NCB.Shared.Models;

    public class InsertNewMessageCommand : BaseCommand
    {
        public int AppId { get; set; }
        public string ConversationId { get; set; }
        public string Content { get; set; }
        public byte ChannelId { get; set; }
        public int MessageId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate { get; set; }

        public override string GetSettingKey() => "Message_Insert";

        public override bool IsValid() => AppId > 0 &&
            !string.IsNullOrWhiteSpace(ConversationId)
            && !string.IsNullOrWhiteSpace(Content)
            && ChannelId > 0;
    }
}