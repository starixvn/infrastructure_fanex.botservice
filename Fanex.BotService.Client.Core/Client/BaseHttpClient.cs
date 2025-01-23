namespace Fanex.BotService.Client.Core.Client
{
    using Configuration;
    using Newtonsoft.Json;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    public class BaseHttpClient
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private readonly IBotSettings botSettings;

        public BaseHttpClient()
        {
            botSettings = BotClientManager.BotSettings;
        }

        public async Task<TOutput> PostAsync<TInput, TOutput>(string requestUrl, TInput data)
        {
            ValidateEndPoint(requestUrl);
            using (var request = CreateRequest(requestUrl, HttpMethod.Post))
            {
                var encodedData = JsonConvert.SerializeObject(data);
                var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
                request.Content = content;
                using (var response = await HttpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None))
                {
                    if (!response.IsSuccessStatusCode)
                    {
                        await ThrowExceptionWhenNotSuccess(response);
                    }

                    var responseData = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<TOutput>(responseData);

                    return result;
                }
            }
        }

        private static void ValidateEndPoint(string endPoint)
        {
            if (string.IsNullOrWhiteSpace(endPoint))
            {
                throw new ArgumentNullException(nameof(endPoint));
            }
        }

        private HttpRequestMessage CreateRequest(string endPoint, HttpMethod method)
        {
            var url = new Uri($"{botSettings.ServiceUrl}{endPoint}");
            var request = new HttpRequestMessage(method, url);
            request.Headers.Accept.Clear();

            return request;
        }

        private static async Task ThrowExceptionWhenNotSuccess(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var message = await response.Content.ReadAsStringAsync();
            var exception = new Exception(message);
            throw exception;
        }
    }
}