namespace Fanex.BotService.Core.Criteria;

using Fanex.BotService.Shared.Models;

public class MigratedToSupergroupCommand : BaseCommand
{
    public string FromConversationId { get; set; }

    public string ToConversationId { get; set; }

    public string UpdatedBy { get; set; }

    public override string GetSettingKey() => "NCB_Conversation_MigratedToSupergroup";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(FromConversationId)
                                      && !string.IsNullOrWhiteSpace(ToConversationId);
}