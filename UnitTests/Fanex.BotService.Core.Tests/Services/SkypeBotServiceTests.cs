namespace Fanex.NCB.BotService.Core.Tests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Fanex.NCB.BotService.Core.Services;
    using Fanex.NCB.Shared.Models;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Integration.AspNet.Core;
    using Microsoft.Bot.Configuration;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Xunit;

    [Obsolete]
    public class SkypeBotServiceTests
    {
        private IHttpClientFactory httpClientFactory;
        private IOptions<SendIndirectMessageSettings> sendIndirectMessageSettings;
        private IOptions<ProxySettings> proxySettings;
        private ILogger<SkypeBotService> filelogger;
        private Logging.ILogger logger;
        private IBotFrameworkHttpAdapter adapter;
        private BotConfiguration botConfig;

        private readonly ConversationInfo FakeConversationInfo = new ConversationInfo
        {
            BotConversationId = "BotId",
            BotName = "BotName",
            ConversationId = "SkypeConversationId"
        };

        [Fact]
        public async Task HttpClientFactory_CallCreateClientWithoutName_WhenNotUseProxy()
        {
            var service = GetService(false, true);
            await service.SendMessageAsync(FakeConversationInfo);
            httpClientFactory.Received().CreateClient();
        }

        [Fact]
        public async Task HttpClientFactory_CallCreateClientWithExpectedtName_WhenUseProxy()
        {
            var service = GetService(true, true);
            await service.SendMessageAsync(FakeConversationInfo);
            httpClientFactory.Received().CreateClient(Arg.Is(NamedHttpClients.ProxiedClient));
        }

        [Fact]
        public async Task SendMessageWithRetryAsync_WhenNoException_ReturnExpectedResult()
        {
            var service = GetService(true, true);
            var actual = await service.SendMessageWithRetryAsync(FakeConversationInfo);
            Assert.Equal(FakeConversationInfo.ConversationId, actual.ConversationId);
            Assert.Equal(FakeConversationInfo.NotificationId, actual.NotificationId);
        }

        [Fact]
        public async Task SendMessageWithRetryAsync_WhenExceptionAndEnableSendIndirect_ReturnExpectedResult()
        {
            var service = GetService(true, true);
            ((BotAdapter)adapter).ContinueConversationAsync(Arg.Any<string>(), Arg.Any<ConversationReference>(), Arg.Any<BotCallbackHandler>(), Arg.Any<CancellationToken>()).Throws(new Exception("Error Message"));
            var actual = await service.SendMessageWithRetryAsync(FakeConversationInfo);
            await logger.Received().ErrorAsync(Arg.Any<string>());
            Assert.Equal(1, actual.ErrorCode);
            Assert.Equal("Error Message", actual.ErrorMessage);
            Assert.Equal(FakeConversationInfo.ConversationId, actual.ConversationId);
            Assert.Equal(FakeConversationInfo.NotificationId, actual.NotificationId);
        }

        private SkypeBotService GetService(bool isUserProxy, bool isEnableSendIndirect)
        {
            httpClientFactory = Substitute.For<IHttpClientFactory>();

            httpClientFactory.CreateClient(Arg.Any<string>()).Returns(new HttpClient());

            sendIndirectMessageSettings = Substitute.For<IOptions<SendIndirectMessageSettings>>();
            proxySettings = Substitute.For<IOptions<ProxySettings>>();
            sendIndirectMessageSettings.Value.Returns(
                new SendIndirectMessageSettings
                {
                    IsEnabled = isEnableSendIndirect,
                    AuthenticateUrl = "Test.com",
                    AppCredentialId = "appId",
                    AppCredentialPassword = "AppPwd",
                    SkypeUrl = "skype.com"
                });

            proxySettings.Value.Returns(new ProxySettings { IsUseProxy = isUserProxy, TimeOut = 10, BypassOnLocal = true, ProxyAddress = "1.1.1.1" });
            filelogger = Substitute.For<ILogger<SkypeBotService>>();
            logger = Substitute.For<Logging.ILogger>();
            adapter = Substitute.For<IBotFrameworkHttpAdapter>();
            botConfig = Substitute.For<BotConfiguration>();
            botConfig.Services = (new List<ConnectedService> { new EndpointService { Name = FakeConversationInfo.BotName } });
            return new SkypeBotService(httpClientFactory, sendIndirectMessageSettings, proxySettings, filelogger, logger, adapter, botConfig);
        }
    }
}