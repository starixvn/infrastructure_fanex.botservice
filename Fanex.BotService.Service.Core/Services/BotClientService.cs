namespace Fanex.BotService.Service.Core.Services;

using System.Net.Http.Json;
using System.Threading.Tasks;
using Configurations;
using Enum;
using Fanex.BotService.Shared.Constants;
using Fanex.BotService.Shared.Models;
using Fanex.BotService.Shared.Models.Message;
using Fanex.BotService.Shared.Services.Interfaces;
using Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Models;
using Newtonsoft.Json;

public class BotClientService(
    IHttpClientFactory httpClientFactory,
    IMemoryCache memoryCache,
    ICommonLogService commonLogService)
    : IBotClientService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(NamedHttpClients.ProxiedClient);
    private readonly BotEndpointConfiguration _botConfigs = AppSettings.BotEndpointConfiguration;
    private const string BotTokenApiKey = "FanexBot_EndpointAPI_Token";

    public async Task<SendMessageResult> SendMessageAsync(BotChannel botChannel, MessageEndpoint message)
    {
        var result = new SendMessageResult
        {
            MessageId = message.MessageId,
            ConversationId = message.ConversationId
        };

        try
        {
            var apiConfig = _botConfigs.ApiConfigs.FirstOrDefault(i => i.Channel == botChannel);
            CheckValidApiConfig(apiConfig);

            //var token = await GetTokenAsync();
            //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var url = new Uri($"{_botConfigs.ServiceUrl}{apiConfig?.SendMessageUrl}");
            using var response = await _httpClient.PostAsJsonAsync(url, message);
            await commonLogService.SentryInfoAsync(JsonConvert.SerializeObject(response));
            response.EnsureSuccessStatusCode();
            var responseData = await response.Content.ReadAsStringAsync();
            await commonLogService.SentryInfoAsync(responseData);

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
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_botConfigs.JwtCacheLifeTime)
        });

        return token;
    }

    private async Task<BotToken> AuthenticateAsync()
    {
        var url = new Uri($"{_botConfigs.ServiceUrl}{_botConfigs.AuthenticatePath}");
        var authData = new { Username = _botConfigs.AppId, Password = _botConfigs.AppPassword };

        using var response = await _httpClient.PostAsJsonAsync(url, authData);
        var responseData = await response.Content.ReadAsStringAsync();
        var token = JsonConvert.DeserializeObject<BotToken>(responseData);
        return token;
    }
}