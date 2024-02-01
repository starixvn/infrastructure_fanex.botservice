namespace Fanex.NCB.Shared.Models
{
    public class CommandResult
    {
        public CommandResult()
        {
            Code = 1;
            Message = "Invalid Data";
        }

        public int Code { get; set; }

        public string Message { get; set; }

        public bool IsSuccessful => Code == 0;
    }
}