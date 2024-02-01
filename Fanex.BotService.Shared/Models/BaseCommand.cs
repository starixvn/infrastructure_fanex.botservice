namespace Fanex.NCB.Shared.Models
{
    using Fanex.Data.Repository;

    public abstract class BaseCommand : NonQueryCommand
    {
        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }
    }
}