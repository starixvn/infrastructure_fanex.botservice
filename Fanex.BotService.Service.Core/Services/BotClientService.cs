namespace Fanex.BotService.Service.Core.Services
{
    using Configurations;
    using Enum;
    using Fanex.BotService.Shared.Constants;
    using Fanex.BotService.Shared.Models.Message;
    using Fanex.BotService.Shared.Services.Interfaces;
    using Fanex.NCB.Shared.Models;
    using Interfaces;
    using Microsoft.Extensions.Caching.Memory;
    using Models;
    using Newtonsoft.Json;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Threading.Tasks;

    public class BotClientService : IBotClientService
    {
        private readonly HttpClient httpClient;
        private readonly BotEndpointConfiguration botConfigs;
        private readonly IMemoryCache memoryCache;
        private readonly ICommonLogService commonLogService;
        private const string BotTokenApiKey = "FanexBot_EndpointAPI_Token";

        public BotClientService(IHttpClientFactory httpClientFactory, IMemoryCache memoryCache,
            ICommonLogService commonLogService)
        {
            httpClient = httpClientFactory.CreateClient(NamedHttpClients.ProxiedClient);
            botConfigs = AppSettings.BotEndpointConfiguration;
            this.memoryCache = memoryCache;
            this.commonLogService = commonLogService;
        }

        public async Task<SendMessageResult> SendMessageAsync(BotChannel botChannel, MessageEndpoint message)
        {
            var result = new SendMessageResult
            {
                MessageId = message.MessageId,
                ConversationId = message.ConversationId
            };

            try
            {
                var apiConfig = botConfigs.ApiConfigs.FirstOrDefault(i => i.Channel == botChannel);
                CheckValidApiConfig(apiConfig);

                var token = await GetTokenAsync();
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var url = new Uri($"{botConfigs.ServiceUrl}{apiConfig?.SendMessageUrl}");
                using var response = await httpClient.PostAsJsonAsync(url, message);
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                result = JsonConvert.DeserializeObject<SendMessageResult>(responseData);
            }
            catch (Exception ex)
            {
                result.MessageId = message.MessageId;
                result.ErrorCode = 1;
                result.SentTime = DateTimeOffset.UtcNow.DateTime;
                result.ErrorMessage = ex.Message;
                await commonLogService.SentryErrorAsync(ex.Message, ex);
            }

            return result;
        }

        private static void CheckValidApiConfig(ApiConfig apiConfig)
        {
            if (apiConfig != null) return;
            throw new ArgumentNullException(nameof(apiConfig));
        }

        private async Task<string> GetTokenAsync()
        {
            var isExistInCache = false;
            var token = string.Empty;
            if (memoryCache.TryGetValue(BotTokenApiKey, out _))
            {
                token = memoryCache.Get<string>(BotTokenApiKey);
                commonLogService.LogFileMessage($"Token from cache: {token}");
                if (!string.IsNullOrWhiteSpace(token))
                {
                    isExistInCache = true;
                }
            }

            if (isExistInCache) return token;

            var authToken = await AuthenticateAsync();
            if (authToken == null) return token;

            token = authToken.AccessToken;
            commonLogService.LogFileMessage($"Token from API: {token}");

            memoryCache.Set(BotTokenApiKey, token, new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(botConfigs.JwtCacheLifeTime)
            });

            return token;
        }

        private async Task<BotToken> AuthenticateAsync()
        {
            var url = new Uri($"{botConfigs.ServiceUrl}{botConfigs.AuthenticatePath}");
            var authData = new Dictionary<string, string>
            {
                { "username", botConfigs.AppId },
                { "password", botConfigs.AppPassword }
            };

            using var authDataContent = new FormUrlEncodedContent(authData);
            using var response = await httpClient.PostAsync(url, authDataContent);
            var responseData = await response.Content.ReadAsStringAsync();
            var token = JsonConvert.DeserializeObject<BotToken>(responseData);
            return token;
        }
    }
}