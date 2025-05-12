namespace Fanex.BotService.Core.Criteria;

using Fanex.BotService.Shared.Models;

public class RemoveBotContactCommand : BaseCommand
{
    public string BotConversationId { get; set; }
    public string ConversationId { get; set; }

    public override string GetSettingKey() => "Conversation_RemoveContact";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(BotConversationId)
                                      && !string.IsNullOrWhiteSpace(ConversationId);
}