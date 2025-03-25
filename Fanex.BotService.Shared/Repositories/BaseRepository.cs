namespace Fanex.BotService.Shared.Repositories
{
    using Data;
    using Data.Repository;
    using Models;
    using System;
    using System.Collections;
    using System.Threading.Tasks;

    public class BaseRepository : IBaseRepository
    {
        public async Task<CommandResult> ExecuteNonQueryAsync(INonQueryCommand command)
        {
            using var db = ObjectDbFactory.CreateInstance(command.GetSettingKey());
            await db.ExecuteNonQueryAsync(command);
            var commandResult = GetCommandResult(db.GetOutputParams());

            return commandResult;
        }

        private static CommandResult GetCommandResult(Hashtable hash)
        {
            var errCode = hash["errCode"];
            var errMsg = hash["errMsg"];

            return new CommandResult
            {
                Code = (errCode is DBNull) ? 0 : Convert.ToInt32(errCode),
                Message = (errMsg is DBNull) ? string.Empty : Convert.ToString(errMsg)
            };
        }
    }
}