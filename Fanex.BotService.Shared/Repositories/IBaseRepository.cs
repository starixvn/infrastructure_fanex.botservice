namespace Fanex.BotService.Shared.Repositories
{
    using Data.Repository;
    using Models;
    using System.Threading.Tasks;

    public interface IBaseRepository
    {
        Task<CommandResult> ExecuteNonQueryAsync(INonQueryCommand command);
    }
}