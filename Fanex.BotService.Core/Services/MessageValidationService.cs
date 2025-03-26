namespace Fanex.BotService.Core.Services
{
    using Caching;
    using Interfaces;
    using Models;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using ILogger = Logging.ILogger;

    public class MessageValidationService(
        CheckDuplicatedMessageLogSetting settings,
        ICacheService cacheService,
        ILogger logger)
        : IMessageValidationService
    {
        public async Task<bool> IsInvalidSendMessageToBotService(string conversationId, string message)
        {
            if (!settings.IsEnabled)
            {
                return false;
            }

            if (settings.ExceptionConversatiionIDs != null && settings.ExceptionConversatiionIDs.Contains(conversationId))
            {
                return false;
            }
            var isInvalid = false;

            try
            {
                var messageKeys = await cacheService.GetAsync<string>(conversationId);
                if (messageKeys != null)
                {
                    var keys = messageKeys.Split(',');

                    foreach (var key in keys)
                    {
                        if (!string.IsNullOrWhiteSpace(key))
                        {
                            var logContent = await cacheService.GetAsync<string>(key);
                            if (logContent == null)
                            {
                                messageKeys = messageKeys.Replace("," + key, string.Empty);
                            }
                            else if (logContent.Equals(message))
                            {
                                isInvalid = true;

                                return isInvalid;
                            }
                        }
                    }

                    await cacheService.SetAsync(conversationId, messageKeys);
                }
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.Message);
            }

            return isInvalid;
        }

        public async Task InsertMessageLogIntoCache(string conversationId, string messageContent, string guid)
        {
            if (!settings.IsEnabled)
            {
                return;
            }

            try
            {
                var messageKeys = await cacheService.GetAsync<string>(conversationId);
                messageKeys = messageKeys + "," + guid;
                await cacheService.SetAsync(conversationId, messageKeys,
                    new CacheItemOptions
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(settings.TimeSpanInMinutes)
                    }
                );

                if (!await cacheService.ContainAsync(guid))
                {
                    await cacheService.SetAsync(
                        guid,
                        messageContent,
                        new CacheItemOptions
                        {
                            RelativeExpiration = TimeSpan.FromMinutes(settings.TimeSpanInMinutes)
                        });
                }
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.Message);
            }
        }

        public async Task RemoveMessageLogOnCache(string guid)
        {
            try
            {
                await cacheService.RemoveAsync(guid);
            }
            catch (Exception ex)
            {
                await logger.ErrorAsync(ex.Message);
            }
        }
    }
}