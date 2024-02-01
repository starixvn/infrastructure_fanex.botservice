namespace Fanex.NCB.Shared.Repositories
{
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NCB.Shared.Models;

    public interface IBaseRepository
    {
        Task<CommandResult> ExecuteNonQueryAsync(INonQueryCommand command);
    }
}