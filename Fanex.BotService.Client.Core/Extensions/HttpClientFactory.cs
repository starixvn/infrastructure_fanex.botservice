namespace Fanex.BotService.Client.Core.Extensions
{
    using System;
    using System.Net.Http;

    internal static class HttpClientFactory
    {
        internal static HttpClient Client { get; }

        static HttpClientFactory()
        {
            var botSettings = BotClientManager.BotSettings;

            var serviceApiUri = new Uri(botSettings.ServiceUrl.TrimEnd('/'));
            Client = new HttpClient()
            {
                BaseAddress = serviceApiUri
            };
        }
    }
}