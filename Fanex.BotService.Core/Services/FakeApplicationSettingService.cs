namespace Fanex.BotService.Core.Services
{
    using Core;
    using Interfaces;
    using Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class FakeApplicationSettingService(IList<BotConfigs> botConfigs) : IApplicationSettingService
    {
        public async Task<IEnumerable<ApplicationSetting>> GetApplicationSettingAsync()
        {
            var applicationSettings = new List<ApplicationSetting>();
            foreach (var botConfig in botConfigs)
            {
                applicationSettings.Add(new ApplicationSetting
                {
                    AppId = botConfig.AppId,
                    MicrosoftAppType = botConfig.MicrosoftAppType,
                    MicrosoftAppId = botConfig.MicrosoftAppId,
                    MicrosoftAppPassword = botConfig.MicrosoftAppPassword,
                    MicrosoftAppTenantId = botConfig.MicrosoftAppTenantId
                });
            }

            return await Task.FromResult(applicationSettings);
        }
    }
}