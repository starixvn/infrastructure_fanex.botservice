namespace Fanex.NCB.BotService.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.BotService.Core.Criteria;
    using Fanex.BotService.Core.Models;
    using Fanex.Data;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;

    public class ApplicationSettingService : IApplicationSettingService
    {
        private readonly IBaseService baseService;
        private readonly IMemoryCache memoryCache;
        private readonly IConfiguration configuration;
        private readonly string applicationSettingKeys = "applicationSettingKeys";

        public ApplicationSettingService(IBaseService baseService,
            IMemoryCache memoryCache,
            IConfiguration configuration)
        {
            this.baseService = baseService;
            this.memoryCache = memoryCache;
            this.configuration = configuration;
        }

        public async Task<IEnumerable<ApplicationSetting>> GetApplicationSettingAsync()
        {
            _ = bool.TryParse(configuration.GetSection("IsReadSettingFromFile")?.Value, out bool isSettingFromFile);
            var isExistInCache = false;
            IEnumerable<ApplicationSetting> applicationSettings = new List<ApplicationSetting>();

            if (memoryCache.TryGetValue(applicationSettingKeys, out _))
            {
                applicationSettings = memoryCache.Get<IEnumerable<ApplicationSetting>>(applicationSettingKeys);
                if (applicationSettings != null)
                {
                    isExistInCache = true;
                }
            }

            if (!isExistInCache)
            {
                if (isSettingFromFile)
                {
                    var applicationSetting = new ApplicationSetting
                    {
                        MicrosoftAppType = configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppTypeKey)?.Value,
                        MicrosoftAppId = configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppIdKey)?.Value,
                        MicrosoftAppPassword = configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppPasswordKey)?.Value,
                        MicrosoftAppTenantId = configuration.GetSection(MicrosoftAppCredentials.MicrosoftAppTenantIdKey)?.Value
                    };

                    applicationSettings = new List<ApplicationSetting> { applicationSetting };
                }
                else
                {
                    var criteria = new GetApplicationSettingCriteria();
                    applicationSettings = await baseService.GetDataAsync<ApplicationSetting>(criteria);
                    foreach (var item in applicationSettings)
                    {
                        var pwd = Cryptography.DecryptPassword(item.MicrosoftAppPassword);
                        item.MicrosoftAppPassword = pwd;
                    }
                }

                memoryCache.Set(applicationSettingKeys, applicationSettings, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            return applicationSettings;
        }
    }
}