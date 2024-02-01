namespace Fanex.NCB.BotService.Core.Criteria
{
    using Fanex.NCB.Shared.Models;

    public class UpdateGroupTopicCommand : BaseCommand
    {
        public string ConversationId { get; set; }

        public string GroupName { get; set; }

        public string UpdatedBy { get; set; }

        public override string GetSettingKey() => "Conversation_UpdateGroupTopic";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(ConversationId)
            && !string.IsNullOrWhiteSpace(GroupName);
    }
}