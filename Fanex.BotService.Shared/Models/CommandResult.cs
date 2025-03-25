namespace Fanex.BotService.Shared.Models
{
    public class CommandResult
    {
        public int Code { get; set; } = 1;

        public string Message { get; set; } = "Invalid Data";

        public bool IsSuccessful => Code == 0;
    }
}