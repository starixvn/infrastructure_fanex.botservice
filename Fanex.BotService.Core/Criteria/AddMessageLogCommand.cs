namespace Fanex.NCB.BotService.Core.Criteria
{
    using System;
    using Fanex.NCB.Shared.Models;

    public class AddMessageLogCommand : BaseCommand
    {
        public int MessageId { get; set; }

        public string ConversationBotId { get; set; }

        public string ConversationId { get; set; }

        public string Content { get; set; }

        public DateTime StartSentTime { get; set; }

        public DateTime EndSentTime { get; set; }

        public bool IsSuccessfull { get; set; }

        public string ErrorMessage { get; set; }

        public override string GetSettingKey() => "MessageLog_Add";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(ConversationId);
    }
}