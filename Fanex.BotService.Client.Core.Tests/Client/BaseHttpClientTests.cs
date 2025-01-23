using Fanex.BotService.Client.Core.Client;
using Fanex.BotService.Client.Core.Configuration;
using Newtonsoft.Json;
using NSubstitute;
using System.Net;

namespace Fanex.BotService.Client.Core.Tests.Client;

public class BaseHttpClientTests
{
    private readonly BaseHttpClient baseHttpClient;
    private readonly IBotSettings botSettings;

    public BaseHttpClientTests()
    {
        botSettings = Substitute.For<IBotSettings>();
        botSettings.ServiceUrl.Returns("http://test.com/");
        botSettings.AppId.Returns(1);
        BotClientManager.RegistryBot(botSettings);
        baseHttpClient = new BaseHttpClient();
    }

    [Fact]
    public async Task PostAsync_ShouldReturnExpectedResult_WhenRequestIsSuccessful()
    {
        // Arrange
        var requestUrl = "api/test";
        var inputData = new { Name = "Test" };
        var expectedOutput = new { Result = "Success" };
        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(expectedOutput))
        });
        var httpClient = new HttpClient(httpMessageHandler);

        // Act
        var result = await baseHttpClient.PostAsync<object, object>(requestUrl, inputData);

        // Assert
        Assert.Equal(expectedOutput.Result, ((dynamic)result).Result);
    }

    [Fact]
    public async Task PostAsync_ShouldThrowException_WhenRequestIsUnsuccessful()
    {
        // Arrange
        var requestUrl = "api/test";
        var inputData = new { Name = "Test" };
        var httpMessageHandler = new MockHttpMessageHandler(new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.BadRequest,
            Content = new StringContent("Bad Request")
        });
        var httpClient = new HttpClient(httpMessageHandler);
        typeof(BaseHttpClient).GetField("HttpClient", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).SetValue(null, httpClient);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(() => baseHttpClient.PostAsync<object, object>(requestUrl, inputData));
        Assert.Equal("Bad Request", exception.Message);
    }

    private class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage response;

        public MockHttpMessageHandler(HttpResponseMessage response)
        {
            this.response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(response);
        }
    }
}