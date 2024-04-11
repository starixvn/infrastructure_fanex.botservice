namespace Fanex.BotService.Core.Services.Interfaces
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.BotService.Core.Models;

    public interface IApplicationSettingService
    {
        Task<IEnumerable<ApplicationSetting>> GetApplicationSettingAsync();
    }
}