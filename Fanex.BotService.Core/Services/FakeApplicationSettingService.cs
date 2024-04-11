namespace Fanex.BotService.Core.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.BotService.Core;
    using Fanex.BotService.Core.Models;
    using Fanex.BotService.Core.Services.Interfaces;

    public class FakeApplicationSettingService : IApplicationSettingService
    {
        private readonly IList<BotConfigs> botConfigs;

        public FakeApplicationSettingService(IList<BotConfigs> botConfigs)
        {
            this.botConfigs = botConfigs;
        }

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