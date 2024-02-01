namespace Fanex.NCB.BotService.EndPoint.Tests.Telegram
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.NCB.BotService.Core;
    using Fanex.NCB.BotService.Core.Criteria;
    using Fanex.NCB.BotService.Core.Services;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.BotService.EndPoint.Controllers.Telegram;
    using Fanex.NCB.BotService.EndPoint.Tests.TestableClasses.Telegram;
    using Fanex.NCB.Shared.Models;
    using global::Telegram.Bot.Types;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using Xunit;

    public class MessageControllerTests
    {
        private readonly IFixture fixture = new Fixture();
        private AppSettings appSettings;
        private ILogger<MessageController> filelogger;
        private Logging.ILogger logger;
        private IConversationService conversationService;
        private ITelegramBotService botService;
        private IMessageLogService messageLogService;

        #region SendMessage

        [Fact]
        public async Task SendMessage_DisableTelgram_ReturnAsExpectedResult()
        {
            // Arrange
            var conversationInfo = fixture.Create<ConversationInfo>();
            var controller = GetController("1");

            // Act
            var result = await controller.SendMessage(conversationInfo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Chatbot service has been stopped due to turning off the configuration by admin.", okResult.Value);

            await botService.DidNotReceive().SendTextMessageAsync(Arg.Any<long>(), Arg.Any<long>(), Arg.Any<string>());
        }

        [Fact]
        public async Task SendMessage_EnableTelgram_ReturnAsExpectedResult()
        {
            // Arrange
            var conversationInfo = new ConversationInfo
            {
                BotConversationId = "1",
                ConversationId = "1",
                Message = "message"
            };
            var controller = GetController();

            // Act
            var result = await controller.SendMessage(conversationInfo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Empty((string)okResult.Value);

            await botService.Received().SendTextMessageAsync(1, 1, conversationInfo.Message);
        }

        [Fact]
        public async Task SendMessage_Exception_ReturnAsExpectedResult()
        {
            // Arrange
            var conversationInfo = new ConversationInfo
            {
                BotConversationId = "1",
                ConversationId = "1",
                Message = "message"
            };
            var exception = fixture.Create<Exception>();
            var controller = GetController();
            botService.SendTextMessageAsync(1, 1, conversationInfo.Message).Returns(Task.FromException<Message>(exception));

            // Act
            var result = await controller.SendMessage(conversationInfo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(exception.Message, okResult.Value);

            controller.Received().Protected("WriteLogAsync", exception, Arg.Any<IDictionary<string, object>>());
        }

        #endregion SendMessage

        #region SendMultiMessage

        [Fact]
        public async Task SendMultiMessage_DisableTelgram_ReturnAsExpectedResult()
        {
            // Arrange
            var notificationMessages = new List<NotificationMessage>
            {
                new NotificationMessage
                {
                    NotificationId = 1,
                    Message = "message",
                    Recipients = new List<ConversationInfo>
                    {
                        new ConversationInfo { BotConversationId = "1", ConversationId = "1" },
                        new ConversationInfo { BotConversationId = "2", ConversationId = "2" }
                    }
                }
            };
            var controller = GetController("1");
            conversationService.GetDistinctNotificationConversation(notificationMessages).Returns(notificationMessages);

            // Act
            var result = await controller.SendMultiMessage(notificationMessages);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var sendMessageResults = Assert.IsType<List<SendMessageResult>>(okResult.Value);

            Assert.Equal(2, sendMessageResults.Count);
            Assert.Collection(sendMessageResults,
                item =>
                {
                    Assert.Equal(1, item.NotificationId);
                    Assert.Equal("1", item.ConversationId);
                    Assert.Equal(1, item.ErrorCode);
                    Assert.Equal("Chatbot service has been stopped due to turning off the configuration by admin.", item.ErrorMessage);
                    Assert.True(item.IsIgnoreRetry);
                },
                item =>
                {
                    Assert.Equal(1, item.NotificationId);
                    Assert.Equal("2", item.ConversationId);
                    Assert.Equal(1, item.ErrorCode);
                    Assert.Equal("Chatbot service has been stopped due to turning off the configuration by admin.", item.ErrorMessage);
                    Assert.True(item.IsIgnoreRetry);
                });
        }

        [Fact]
        public async Task SendMultiMessage_EnableTelgram_ReturnAsExpectedResult()
        {
            // Arrange
            var notificationMessages = new List<NotificationMessage>
            {
                new NotificationMessage
                {
                    NotificationId = 1,
                    Message = "message",
                    Recipients = new List<ConversationInfo>
                    {
                        new ConversationInfo { BotConversationId = "1", ConversationId = "1" },
                        new ConversationInfo { BotConversationId = "2", ConversationId = "2" }
                    }
                }
            };
            var sendMessageResult = fixture.Create<SendMessageResult>();
            var controller = GetController();
            conversationService.GetDistinctNotificationConversation(notificationMessages).Returns(notificationMessages);
            controller.Protected("SendMessageAsync", Arg.Any<ConversationInfo>())
                .Returns(Task.FromResult(sendMessageResult));

            // Act
            var result = await controller.SendMultiMessage(notificationMessages);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var sendMessageResults = Assert.IsType<List<SendMessageResult>>(okResult.Value);

            Assert.Equal(2, sendMessageResults.Count);
            Assert.All(sendMessageResults, item => Assert.Equal(sendMessageResult, item));
        }

        #endregion SendMultiMessage

        #region SendMessageAsync

        [Fact]
        public async Task SendMessageAsync_Duplicate_ReturnAsExpectedResult()
        {
            // Arrange
            var conversationInfo = fixture.Create<ConversationInfo>();
            conversationInfo.IsDuplicated = true;
            var controller = GetController();
            messageLogService.IsInvalidSendMessageToBotService(conversationInfo.ConversationId, conversationInfo.Message).Returns(true);

            // Act
            var result = await controller.Public_SendMessageAsync(conversationInfo);

            // Assert
            Assert.Equal(conversationInfo.ConversationId, result.ConversationId);
            Assert.Equal(-1, result.ErrorCode);
            Assert.Equal("Error occurred due to duplicate content in a pre-defined period of time.", result.ErrorMessage);
            Assert.Equal(conversationInfo.NotificationId, result.NotificationId);
            Assert.True(result.IsIgnoreRetry);
        }

        [Fact]
        public async Task SendMessageAsync_Exception_ReturnAsExpectedResult()
        {
            // Arrange
            var conversationInfo = fixture.Create<ConversationInfo>();
            conversationInfo.IsDuplicated = false;
            var exception = fixture.Create<Exception>();
            var controller = GetController();
            messageLogService.IsInvalidSendMessageToBotService(conversationInfo.ConversationId, conversationInfo.Message).Returns(false);
            messageLogService.InsertMessageLogIntoCache(conversationInfo.ConversationId, conversationInfo.Message, Arg.Any<string>()).Returns(Task.FromException(exception));

            // Act
            var result = await controller.Public_SendMessageAsync(conversationInfo);

            // Assert
            Assert.Equal(conversationInfo.ConversationId, result.ConversationId);
            Assert.Equal(1, result.ErrorCode);
            Assert.Equal(exception.Message, result.ErrorMessage);
            Assert.Equal(conversationInfo.NotificationId, result.NotificationId);
            Assert.False(result.IsIgnoreRetry);

            await messageLogService.Received().RemoveMessageLogOnCache(conversationInfo.ConversationId, Arg.Any<string>());
            controller.Received().Protected("WriteLogAsync", exception, Arg.Any<IDictionary<string, object>>());
        }

        ////[Fact]
        ////public async Task SendMessageAsync_SendMessageSuccessfull_ReturnAsExpectedResult()
        ////{
        ////    // Arrange
        ////    var conversationInfo = new ConversationInfo
        ////    {
        ////        BotConversationId = "1",
        ////        ConversationId = "1"
        ////    };
        ////    var notificationId = fixture.Create<int>();
        ////    var messageContent = fixture.Create<string>();
        ////    var sendMessageResult = new SendMessageResult { ErrorCode = 1 };
        ////    var controller = GetController();
        ////    messageLogService.IsInvalidSendMessageToBotService(conversationInfo.ConversationId, messageContent).Returns(false);
        ////    controller.Protected("SendTextMessageAsync", 1, 1, notificationId, messageContent).Returns(sendMessageResult);

        ////    // Act
        ////    var result = await controller.Public_SendMessageAsync(conversationInfo, notificationId, messageContent);

        ////    // Assert
        ////    Assert.Equal(sendMessageResult, result);

        ////    await messageLogService.Received().InsertMessageLogIntoCache(conversationInfo.ConversationId, messageContent, Arg.Any<string>());
        ////    await messageLogService.Received().RemoveMessageLogOnCache(conversationInfo.ConversationId, Arg.Any<string>());
        ////}

        #endregion SendMessageAsync

        #region AddMessageLogToDatabaseAsync

        [Fact]
        public async Task AddMessageLogToDatabaseAsync_WhenCalled_CallCorrectlyMethod()
        {
            // Arrange
            var conversationInfo = fixture.Create<ConversationInfo>();
            var result = fixture.Create<SendMessageResult>();
            var command = new AddMessageLogCommand();
            var commandResult = fixture.Create<CommandResult>();
            var controller = GetController();
            conversationService.AddMessageLog(Arg.Do<AddMessageLogCommand>(x => command = x)).Returns(commandResult);

            // Act
            await controller.Public_AddMessageLogToDatabaseAsync(conversationInfo, result);

            // Assert
            Assert.Equal(conversationInfo.BotConversationId, command.FromConversationId);
            Assert.Equal(conversationInfo.ConversationId, command.ToConversationId);
            Assert.Equal(conversationInfo.Message, command.Content);
            Assert.Equal(result.NotificationId, command.ReferenceId);
            Assert.Equal(result.ErrorCode == 0, command.IsSuccessfull);
            Assert.Equal(result.ErrorMessage, command.ErrorMessage);
        }

        [Fact]
        public async Task AddMessageLogToDatabaseAsync_Exception_CallCorrectlyMethod()
        {
            // Arrange
            var conversationInfo = fixture.Create<ConversationInfo>();
            var result = fixture.Create<SendMessageResult>();
            var exception = fixture.Create<Exception>();
            var controller = GetController();
            conversationService.AddMessageLog(Arg.Any<AddMessageLogCommand>()).Returns(Task.FromException<CommandResult>(exception));

            // Act
            await controller.Public_AddMessageLogToDatabaseAsync(conversationInfo, result);

            // Assert
            controller.Received().Protected("WriteLogAsync", exception, Arg.Any<IDictionary<string, object>>());
        }

        #endregion AddMessageLogToDatabaseAsync

        [Fact]
        public async Task WriteLogAsync_WhenCalled_CallCorrectlyMethod()
        {
            // Arrange
            var exception = fixture.Create<Exception>();
            var controller = GetController();

            // Act
            await controller.Public_WriteLogAsync(exception);

            // Assert
            await logger.Received().ErrorAsync(Arg.Is<string>(x => x.Contains(exception.Message)));
        }

        #region Helpers

        private MessageControllerTestable GetController(string enableChannelIds = "1,2")
        {
            appSettings = new AppSettings { EnableChannelIds = enableChannelIds };
            var settings = Substitute.For<IOptions<AppSettings>>();
            settings.Value.Returns(appSettings);

            filelogger = Substitute.For<ILogger<MessageController>>();
            logger = Substitute.For<Logging.ILogger>();
            botService = Substitute.For<ITelegramBotService>();
            conversationService = Substitute.For<IConversationService>();
            messageLogService = Substitute.For<IMessageLogService>();
            return Substitute.ForPartsOf<MessageControllerTestable>(settings, filelogger, logger, conversationService, botService, messageLogService);
        }

        #endregion Helpers
    }
}