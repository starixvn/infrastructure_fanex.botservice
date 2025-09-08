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
using Newtonsoft.Json;

public class BotClientService(
    IHttpClientFactory httpClientFactory,
    ICommonLogService commonLogService)
    : IBotClientService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(NamedHttpClients.ProxiedClient);
    private readonly BotEndpointConfiguration _botConfigs = AppSettings.BotEndpointConfiguration;

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
}