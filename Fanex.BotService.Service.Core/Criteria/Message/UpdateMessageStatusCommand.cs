namespace Fanex.BotService.Service.Core.Criteria.Message
{
    using Fanex.NCB.Shared.Models;

    public class UpdateMessageStatusCommand : BaseCommand
    {
        public long MessageId { get; set; }
        public string Status { get; set; }

        public override string GetSettingKey() => "MessageStatus_Update";

        public override bool IsValid() => MessageId > 0 && !string.IsNullOrWhiteSpace(Status);
    }
}