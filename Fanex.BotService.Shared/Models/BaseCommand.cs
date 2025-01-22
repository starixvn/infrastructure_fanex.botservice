namespace Fanex.BotService.Shared.Models
{
    using Data.Repository;

    public abstract class BaseCommand : NonQueryCommand
    {
        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }
    }
}