namespace Fanex.BotService.Core.Services.Interfaces
{
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IApplicationSettingService
    {
        Task<IEnumerable<ApplicationSetting>> GetApplicationSettingAsync();
    }
}