namespace Fanex.NCB.BotService.EndPoint.Tests
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.NCB.BotService.Core;
    using Fanex.NCB.BotService.Core.Services;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.BotService.EndPoint.Controllers;
    using Fanex.NCB.Shared.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Bot.Schema;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using NSubstitute;
    using Xunit;

    public class NotifyControllerTests
    {
        private AppSettings appSettings;
        private IMessageLogService messageLogService;
        private IConversationService conversationService;
        private ILogger<NotifyController> filelogger;
        private Logging.ILogger logger;
        private ISkypeBotService skypeBotService;

     
        #region Send

        [Fact]
        public async Task Send_DisableSkype_ReturnAsExpectedResult()
        {
            // Arrange
            var conversationInfo = new ConversationInfo();
            var controller = GetController("2");

            // Act
            var result = await controller.Send(conversationInfo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Chatbot service has been stopped due to turning off the configuration by admin.", okResult.Value);
        }

        [Fact]
        public async Task Send_EnableSkype_ReturnAsExpectedResult()
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
            var result = await controller.Send(conversationInfo);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Empty((string)okResult.Value);

            await skypeBotService.Received().SendMessageAsync(conversationInfo);
        }

        #endregion Send

        [Fact]
        public async Task SendMulti_NotificationListIsNull_ReturnObjectResultWithEmptyList()
        {
            var controller = GetController();

            //Actual
            var actual = await controller.SendMulti(null);

            //Assert
            Assert.IsType<OkObjectResult>(actual);
            Assert.Empty((List<SendMessageResult>)actual.Value);
        }

        [Fact]
        public async Task SendMulti_NotificationListIsEmpty_ReturnObjectResultWithEmptyList()
        {
            var controller = GetController();
            var notificationMessage = new List<NotificationMessage> { new NotificationMessage { NotificationId = 1, Recipients = new List<ConversationInfo>() } };

            //Actual
            var actual = await controller.SendMulti(notificationMessage);

            //Assert
            Assert.IsType<OkObjectResult>(actual);
            Assert.Empty((List<SendMessageResult>)actual.Value);
        }

        [Fact]
        public async Task SendMulti_NotificationListIsNotEmpty_ReturnObjectResultWithSuccessList()
        {
            var controller = GetController();

            var notificationMessage = new List<NotificationMessage>
            {
                new NotificationMessage {
                NotificationId = 1,
                Message = "test",
                Recipients = new List<ConversationInfo>
                    {
                        new ConversationInfo
                        {
                            ConversationId = "ConversationId"
                        }
                    }
                }
            };
            conversationService.GetDistinctNotificationConversation(Arg.Any<List<NotificationMessage>>()).Returns(notificationMessage);
            skypeBotService.SendMessageWithRetryAsync(Arg.Any<ConversationInfo>()).Returns(new SendMessageResult { NotificationId = 1, ConversationId = "ConversationId" });
            //Actual
            var actual = await controller.SendMulti(notificationMessage);

            //Assert
            Assert.IsType<OkObjectResult>(actual);
            Assert.Single((List<SendMessageResult>)actual.Value);
            Assert.Equal(1, ((List<SendMessageResult>)actual.Value)[0].NotificationId);
            Assert.Equal("ConversationId", ((List<SendMessageResult>)actual.Value)[0].ConversationId);
            Assert.Equal(0, ((List<SendMessageResult>)actual.Value)[0].ErrorCode);
            Assert.Null(((List<SendMessageResult>)actual.Value)[0].ErrorMessage);
        }

        [Fact]
        public async Task SendMulti_NotificationListIsNotEmpty_ReturnObjectResultWithErrorList()
        {
            var controller = GetController();

            var notificationMessage = new List<NotificationMessage>
            {
                new NotificationMessage {
                NotificationId = 1,
                Message = "test",
                Recipients = new List<ConversationInfo>
                    {
                        new ConversationInfo()
                    }
                }
            };
            conversationService.GetDistinctNotificationConversation(Arg.Any<List<NotificationMessage>>()).Returns(notificationMessage);
            skypeBotService.SendMessageWithRetryAsync(Arg.Any<ConversationInfo>())
                .Returns(new SendMessageResult { NotificationId = 1, ConversationId = "ConversationId", ErrorCode = 1, ErrorMessage = "error msg" });

            //Actual
            var actual = await controller.SendMulti(notificationMessage);

            //Assert
            Assert.IsType<OkObjectResult>(actual);
            Assert.Single((List<SendMessageResult>)actual.Value);
            Assert.Equal(1, ((List<SendMessageResult>)actual.Value)[0].NotificationId);
            Assert.Equal(1, ((List<SendMessageResult>)actual.Value)[0].ErrorCode);
            Assert.Equal("error msg", ((List<SendMessageResult>)actual.Value)[0].ErrorMessage);
        }

        private NotifyController GetController(string enableChannelIds = "1,2")
        {
            appSettings = new AppSettings { EnableChannelIds = enableChannelIds };
            var appSettingOptions = Substitute.For<IOptions<AppSettings>>();
            appSettingOptions.Value.Returns(appSettings);

            filelogger = Substitute.For<ILogger<NotifyController>>();
            logger = Substitute.For<Logging.ILogger>();
            messageLogService = Substitute.For<IMessageLogService>();
            conversationService = Substitute.For<IConversationService>();
            skypeBotService = Substitute.For<ISkypeBotService>();
            return new NotifyController(appSettingOptions, filelogger, logger, messageLogService, conversationService, skypeBotService);
        }
    }
}