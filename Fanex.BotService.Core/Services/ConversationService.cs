namespace Fanex.BotService.Core.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;
    using Fanex.BotService.Core.Criteria;
    using Fanex.BotService.Core.Models;
    using Fanex.BotService.Core.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Connector;
    using Microsoft.Bot.Connector.Authentication;
    using Microsoft.Extensions.Caching.Memory;
    using Microsoft.Extensions.Configuration;

    public class ConversationService : IConversationService
    {
        private readonly string botSettingKeys = "BotSettingKeys";
        private readonly IBaseService baseService;
        private readonly IMemoryCache memoryCache;
        private readonly AppSettings appSettings;

        public ConversationService(IBaseService baseService,
            AppSettings appSettings,
            IMemoryCache memoryCache)
        {
            this.baseService = baseService;
            this.memoryCache = memoryCache;
            this.appSettings = appSettings;
        }

        public async Task<IEnumerable<ApplicationSetting>> GetApplicationSettingAsync()
        {
            var criteria = new GetApplicationSettingCriteria();
            var applicationSettings = await baseService.GetDataAsync<ApplicationSetting>(criteria);
            return applicationSettings;
        }

        public async Task<IEnumerable<Bot>> GetBotAsync()
        {
            var isExistInCache = false;
            IEnumerable<Bot> bots = new List<Bot>();
            if (memoryCache.TryGetValue(botSettingKeys, out _))
            {
                bots = memoryCache.Get<IEnumerable<Bot>>(botSettingKeys);
                if (bots != null)
                {
                    isExistInCache = true;
                }
            }

            if (!isExistInCache)
            {
                var criteria = new GetAllBotCriteria();
                bots = await baseService.GetDataAsync<Bot>(criteria);
                memoryCache.Set(botSettingKeys, bots, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(appSettings.BotListCacheLifeTime)
                });
            }

            return bots;
        }

        public async Task<CommandResult> AddBotContactAsync(ITurnContext turnContext)
        {
            var microsoftAppId = ((AppCredentials)((ConnectorClient)turnContext.TurnState.ToList()[1].Value).Credentials).MicrosoftAppId;

            var command = new AddBotContactCommand
            {
                BotConversationId = turnContext.Activity?.Recipient.Id,
                BotName = turnContext.Activity?.Recipient.Name,
                ConversationId = turnContext.Activity?.Conversation.Id,
                ChannelName = turnContext.Activity?.ChannelId,
                DisplayName = turnContext.Activity?.From.Name,
                ServiceUrl = turnContext.Activity?.ServiceUrl,
                IsGroup = turnContext.Activity?.Conversation.IsGroup ?? true,
                MicrosoftAppId = microsoftAppId
            };

            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> RemoveBotContactAsync(ITurnContext turnContext)
        {
            var command = new RemoveBotContactCommand
            {
                BotConversationId = turnContext.Activity?.Recipient.Id,
                ConversationId = turnContext.Activity?.Conversation.Id
            };

            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> AddBotToGroupAsync(ITurnContext turnContext)
        {
            var microsoftAppId = ((AppCredentials)((ConnectorClient)turnContext.TurnState.ToList()[1].Value).Credentials).MicrosoftAppId;

            var command = new AddBotToGroupCommand
            {
                BotConversationId = turnContext.Activity?.Recipient.Id,
                BotName = turnContext.Activity?.Recipient.Name,
                ConversationId = turnContext.Activity?.Conversation.Id,
                GroupName = turnContext.Activity?.TopicName,
                ChannelName = turnContext.Activity?.ChannelId,
                ServiceUrl = turnContext.Activity?.ServiceUrl,
                IsGroup = turnContext.Activity?.Conversation.IsGroup ?? false,
                UpdatedBy = turnContext.Activity?.From.Id,
                MicrosoftAppId = microsoftAppId
            };

            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> RemoveBotOutGroupAsync(ITurnContext turnContext)
        {
            var command = new RemoveBotOutGroupCommand
            {
                BotConversationId = turnContext.Activity?.Recipient.Id,
                ConversationId = turnContext.Activity?.Conversation.Id,
                UpdatedBy = turnContext.Activity?.From.Id
            };

            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> UpdateGroupTopicAsync(ITurnContext turnContext)
        {
            var command = new UpdateGroupTopicCommand
            {
                ConversationId = turnContext.Activity?.Conversation.Id,
                GroupName = HttpUtility.HtmlDecode(turnContext.Activity?.TopicName),
                UpdatedBy = turnContext.Activity?.From.Id
            };

            return await baseService.ExecuteNonQueryAsync(command);
        }

        public async Task<CommandResult> MigratedToSupergroupAsync(MigratedToSupergroupCommand command)
            => await baseService.ExecuteNonQueryAsync(command);

        public async Task<CommandResult> AddMessageLog(AddMessageLogCommand command)
            => await baseService.ExecuteNonQueryAsync(command);
    }
}