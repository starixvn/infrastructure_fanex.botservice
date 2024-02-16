namespace Fanex.BotService.EndPoint.BackgroundJobs
{
    using System;
    using System.Threading.Tasks;
    using Fanex.BotService.Core.Criteria;
    using Fanex.BotService.EndPoint.Models;
    using Fanex.BotService.Shared.Services.Interfaces;
    using Fanex.NCB.BotService.Core.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Extensions.Caching.Memory;

    public class RefreshListBotOnMemcacheAutoJob : BaseWorker
    {
        private readonly string botSettingKeys = "BotSettingKeys";
        private readonly IBaseService baseService;
        private readonly IMemoryCache memoryCache;

        public RefreshListBotOnMemcacheAutoJob(IBaseService baseService,
            IMemoryCache memoryCache,
            JobSettings jobSetting,
            ICommonLogService logService) : base(logService)
        {
            this.baseService = baseService;
            this.memoryCache = memoryCache;
            this.jobSetting = jobSetting;
        }

        public override async Task ExecuteJobAsync()
        {
            var bots = await baseService.GetDataAsync<Bot>(new GetAllBotCriteria());
            memoryCache.Set(botSettingKeys, bots, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(jobSetting.CacheLiveTimeInMinute)
            });
        }
    }
}