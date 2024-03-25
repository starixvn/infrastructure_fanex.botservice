namespace Fanex.BotService.Client.Core.Client
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.BotService.Client.Core.Configuration;
    using Newtonsoft.Json;

    internal class BaseHttpClient
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private readonly IBotSettings botSettings;

        public BaseHttpClient()
        {
            botSettings = BotClientManager.BotSettings;
        }

        protected T Get<T>(string requestUrl)
        {
            ValidateEndPoint(requestUrl);
            using (var request = CreateRequest(requestUrl, HttpMethod.Get))
            {
                using (var response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None).Result)
                {
                    ThrowExceptionWhenNotSuccess(response);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        var result = JsonConvert.DeserializeObject<T>(responseData);

                        return result;
                    }

                    return default;
                }
            }
        }

        protected async Task<T> GetAsync<T>(string requestUrl)
        {
            ValidateEndPoint(requestUrl);
            using (var request = CreateRequest(requestUrl, HttpMethod.Get))
            {
                using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None))
                {
                    ThrowExceptionWhenNotSuccess(response);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<T>(responseData);

                        return result;
                    }

                    return default;
                }
            }
        }

        protected TOutput Post<TInput, TOutput>(string requestUrl, TInput data)
        {
            ValidateEndPoint(requestUrl);
            using (var request = CreateRequest(requestUrl, HttpMethod.Post))
            {
                var encodedData = JsonConvert.SerializeObject(data);
                var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
                request.Content = content;
                using (var response = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None).Result)
                {
                    ThrowExceptionWhenNotSuccess(response);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = response.Content.ReadAsStringAsync().Result;
                        var result = JsonConvert.DeserializeObject<TOutput>(responseData);

                        return result;
                    }

                    return default;
                }
            }
        }

        protected async Task<TOutput> PostAsync<TInput, TOutput>(string requestUrl, TInput data)
        {
            ValidateEndPoint(requestUrl);
            using (var request = CreateRequest(requestUrl, HttpMethod.Post))
            {
                var encodedData = JsonConvert.SerializeObject(data);
                var content = new StringContent(encodedData, Encoding.UTF8, "application/json");
                request.Content = content;
                using (var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None))
                {
                    ThrowExceptionWhenNotSuccess(response);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseData = await response.Content.ReadAsStringAsync();
                        var result = JsonConvert.DeserializeObject<TOutput>(responseData);

                        return result;
                    }

                    return default;
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
            Uri url = new Uri($"{botSettings.ServiceUrl}{endPoint}");
            var request = new HttpRequestMessage(method, url);
            request.Headers.Accept.Clear();

            return request;
        }

        private void ThrowExceptionWhenNotSuccess(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return;
                }

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException();
                }

                var message = response.Content.ReadAsStringAsync().Result;
                var exception = new Exception(message);
                throw exception;
            }
        }
    }
}