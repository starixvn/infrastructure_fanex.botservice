namespace Fanex.BotService.Core.Services
{
    using Criteria;
    using Data;
    using Fanex.BotService.Shared.Services.Interfaces;
    using Interfaces;
    using Microsoft.Extensions.Caching.Memory;
    using Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class ApplicationSettingService(IBaseService baseService, IMemoryCache memoryCache) : IApplicationSettingService
    {
        private const string ApplicationSettingKeys = "applicationSettingKeys";

        public async Task<IEnumerable<ApplicationSetting>> GetApplicationSettingAsync()
        {
            var isExistInCache = false;
            IEnumerable<ApplicationSetting> applicationSettings = new List<ApplicationSetting>();

            if (memoryCache.TryGetValue(ApplicationSettingKeys, out _))
            {
                applicationSettings = memoryCache.Get<IEnumerable<ApplicationSetting>>(ApplicationSettingKeys);
                if (applicationSettings != null)
                {
                    isExistInCache = true;
                }
            }

            if (isExistInCache) return applicationSettings;

            var criteria = new GetApplicationSettingCriteria();
            applicationSettings = await baseService.GetDataAsync<ApplicationSetting>(criteria);
            foreach (var item in applicationSettings)
            {
                var pwd = Cryptography.DecryptPassword(item.MicrosoftAppPassword);
                item.MicrosoftAppPassword = pwd;
            }

            memoryCache.Set(ApplicationSettingKeys, applicationSettings, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            return applicationSettings;
        }
    }
}