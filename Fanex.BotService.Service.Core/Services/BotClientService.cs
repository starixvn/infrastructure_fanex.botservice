namespace Fanex.BotService.Service.Core.Services
{
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using Fanex.BotService.Service.Core.Configurations;
    using Fanex.BotService.Service.Core.Enum;
    using Fanex.BotService.Service.Core.Models;
    using Fanex.BotService.Service.Core.Services.Interfaces;
    using Fanex.BotService.Shared.Models.Message;
    using Fanex.Caching;
    using Fanex.NCB.Shared.Models;
    using Newtonsoft.Json;

    public class BotClientService : IBotClientService
    {
        private readonly HttpClient httpClient;
        private readonly BotEndpointConfiguration botConfigs;
        private readonly ICacheService cacheService;
        private const string BOT_TOKEN_API_KEY = "FanexBot_EndpointAPI_Token";

        public BotClientService(IHttpClientFactory httpClientFactory, ICacheService cacheService)
        {
            httpClient = httpClientFactory.CreateClient();
            botConfigs = AppSettings.BotEndpointConfiguration;
            this.cacheService = cacheService;
        }

        public async Task<SendMessageResult> SendMessageAsync(BotChannel botChannel, Message message)
        {
            ApiConfig apiConfig = botConfigs.ApiConfigs.FirstOrDefault(i => i.Channel == botChannel);
            string token = await GetTokenAsync();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var url = new Uri($"{botConfigs.ServiceUrl}{apiConfig.SendMessageUrl}");
            using (var response = await httpClient.PostAsJsonAsync(url, message))
            {
                response.EnsureSuccessStatusCode();
                var responseData = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SendMessageResult>(responseData);
                return result;
            }
        }

        private async Task<string> GetTokenAsync()
        {
            string token = await cacheService.GetAsync<string>(BOT_TOKEN_API_KEY);
            if (string.IsNullOrWhiteSpace(token))
            {
                var authToken = await AuthenticateAsync();
                if (authToken != null)
                {
                    await cacheService.SetAsync(BOT_TOKEN_API_KEY, authToken.AccessToken, new CacheItemOptions
                    {
                        AbsoluteExpiration = authToken.Expires.Value.AddMinutes(-5)
                    });
                }
            }
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