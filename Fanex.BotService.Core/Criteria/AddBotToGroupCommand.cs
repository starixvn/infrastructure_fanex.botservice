namespace Fanex.NCB.BotService.Core.Criteria
{
    using Fanex.NCB.Shared.Models;

    public class AddBotToGroupCommand : BaseCommand
    {
        public string BotConversationId { get; set; }
        public string BotName { get; set; }
        public string ConversationId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string ChannelName { get; set; }
        public string ServiceUrl { get; set; }

        public string MicrosoftAppId { get; set; }
        public string UpdatedBy { get; set; }

        public override string GetSettingKey() => "Conversation_AddBotToGroup";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(BotConversationId)
            && !string.IsNullOrWhiteSpace(ConversationId)
            && !string.IsNullOrWhiteSpace(ChannelName);
    }
}