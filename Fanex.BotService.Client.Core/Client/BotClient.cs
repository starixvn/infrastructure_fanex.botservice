namespace Fanex.BotService.Client.Core.Client
{
    using Extensions;
    using Models;
    using Newtonsoft.Json;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    public static class BotClient
    {
        public static async Task SendAsync(Conversation conversation)
            => await PostAsync<Conversation, Task>("/api/bot/send", conversation);

        private static async Task PostAsync<TInput, TOutput>(string requestUrl, TInput data)
        {
            ValidateEndPoint(requestUrl);

            var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
            using (var response = await HttpClientFactory.Client.PostAsync(requestUrl, content))
            {
                if (!response.IsSuccessStatusCode)
                {
                    await ThrowExceptionWhenNotSuccess(response);
                }

                var responseData = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<TOutput>(responseData);

                return;
            }
        }

        private static void ValidateEndPoint(string endPoint)
        {
            if (string.IsNullOrWhiteSpace(endPoint))
            {
                throw new ArgumentNullException(nameof(endPoint));
            }
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