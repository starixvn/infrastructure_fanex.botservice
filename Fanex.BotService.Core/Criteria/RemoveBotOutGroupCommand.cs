namespace Fanex.BotService.Core.Criteria
{
    using Fanex.BotService.Shared.Models;

    public class RemoveBotOutGroupCommand : BaseCommand
    {
        public string BotConversationId { get; set; }

        public string ConversationId { get; set; }

        public string UpdatedBy { get; set; }

        public override string GetSettingKey() => "Conversation_RemoveBotOutGroup";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(BotConversationId)
            && !string.IsNullOrWhiteSpace(ConversationId);
    }
}