namespace Fanex.NCB.BotService.Core.Tests.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Caching;
    using Fanex.Logging;
    using Fanex.NCB.BotService.Core.Enum;
    using Fanex.NCB.BotService.Core.Models;
    using Fanex.NCB.BotService.Core.Services;
    using Newtonsoft.Json;
    using NSubstitute;
    using Xunit;

    public class MessageLogSerrviceTests
    {
        private readonly CheckDuplicatedMessageLogSetting settings = new() { IsEnabled = true, TimeSpanInMinutes = 5 };
        private ICacheService cacheService;
        private IRedisCacheExtendService cacheExtendService;
        private ILogger logger;

        [Fact]
        public void IsInvalidSendMessageToBotService_IsDisbaledSetting_ReturnFalse()
        {
            settings.IsEnabled = false;
            var service = GetService();
            var actual = service.IsInvalidSendMessageToBotService("ABC", "messsage");
            Assert.False(actual);
            cacheService.DidNotReceive().GetAsync<string>(Arg.Any<string>());
        }

        [Fact]
        public void IsInvalidSendMessageToBotService_OnExceptionList_ReturnFalse()
        {
            settings.ExceptionConversatiionIDs = new[] { "ABC" };
            var service = GetService();
            var actual = service.IsInvalidSendMessageToBotService("ABC", "messsage");
            Assert.False(actual);
            cacheService.DidNotReceive().GetAsync<string>(Arg.Any<string>());
        }

        [Fact]
        public void IsInvalidSendMessageToBotService_WhenExistedOnCache_ReturnFalse()
        {
            settings.ExceptionConversatiionIDs = new[] { "ABC" };
            var service = GetService();
            var logMsg = new ConversationMessageLog { ConversationId = "ABC", Message = "Msg", Status = SentMessageStatus.Success };
            cacheService.GetAsync<string>(Arg.Is("ABC")).Returns(JsonConvert.SerializeObject(logMsg));

            var actual = service.IsInvalidSendMessageToBotService("ABC", "Msg");
            Assert.False(actual);
        }

        [Fact]
        public void IsInvalidSendMessageToBotService_WhenNotExistedOnCache_ReturnTrue()
        {
            var service = GetService();
            var logMsg = new ConversationMessageLog { ConversationId = "ABC", Message = "Msg", Status = SentMessageStatus.Success };
            cacheExtendService.GetValueByKeyContains<string>(Arg.Is(logMsg.ConversationId)).Returns(new List<string> { logMsg.Message });

            var actual = service.IsInvalidSendMessageToBotService("ABC", "Msg");
            Assert.True(actual);
        }

        [Fact]
        public async Task InsertMessageLogIntoCache_WhenCalled_InsertOnCache()
        {
            var conversationLogMsg = new ConversationMessageLog { ConversationId = "A:1234", Message = "ABC", Status = SentMessageStatus.Success };
            var service = GetService();
            await service.InsertMessageLogIntoCache(conversationLogMsg.ConversationId, conversationLogMsg.Message, "Guid");
            await cacheService.Received().SetAsync(Arg.Do<string>(x => x.Contains("A-1234")),
                Arg.Is(conversationLogMsg.Message),
                Arg.Any<CacheItemOptions>());
        }

        private MessageLogService GetService()
        {
            cacheService = Substitute.For<ICacheService>();
            cacheExtendService = Substitute.For<IRedisCacheExtendService>();
            logger = Substitute.For<ILogger>();
            return new MessageLogService(settings, cacheService, cacheExtendService, logger);
        }
    }
}