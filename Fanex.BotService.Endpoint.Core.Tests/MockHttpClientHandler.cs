namespace Fanex.BotService.Endpoint.Core.Tests;

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

public class MockHttpMessageHandler : DelegatingHandler
{
    private readonly HttpResponseMessage httpResponseMessage;

    public MockHttpMessageHandler(HttpResponseMessage responseMessage)
    {
        httpResponseMessage = responseMessage;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return await Task.FromResult(httpResponseMessage);
    }
}