using System;
using Fanex.NCB.BotService.Core.Enum;

namespace Fanex.NCB.BotService.Core.Models
{
    public class ConversationMessageLog
    {
        public string ConversationId { get; set; }

        public string Message { get; set; }

        public DateTime? SentDate { get; set; }

        public SentMessageStatus Status { get; set; }

        public bool IsSuccessful => Status == SentMessageStatus.Success;
    }
}