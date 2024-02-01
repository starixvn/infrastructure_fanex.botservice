namespace Fanex.NCB.Shared.Models
{
    using System;

    public class SendMessageResult
    {
        public int MessageId { get; set; }

        public string ConversationId { get; set; }

        public DateTime SentTime { get; set; }

        public int ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public bool IsIgnoreRetry { get; set; }

        public bool IsSucessfull
        {
            get
            {
                return ErrorCode == 0;

            }
        }
    }
}