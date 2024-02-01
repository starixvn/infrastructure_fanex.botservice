namespace Fanex.NCB.BotService.Core.Tests.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Fanex.NCB.BotService.Core.Criteria;
    using Fanex.NCB.BotService.Core.Models;
    using Fanex.NCB.BotService.Core.Services;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Services.Interfaces;
    using NSubstitute;
    using Xunit;

    public class ConversationServiceTests
    {
        private IBaseService baseService;
        private CheckDuplicatedMessageLogSetting settings;

        [Fact]
        public async Task AddBotToGroup_WhenCalled_ExpectedCallExecuteNonQueryAsync()
        {
            var criteria = new AddBotToGroupCommand
            {
                BotConversationId = "aa",
                ConversationId = "bb"
            };
            var commandResult = new CommandResult { Code = 0 };
            var service = GetService();

            baseService.ExecuteNonQueryAsync(Arg.Do<AddBotToGroupCommand>(x => criteria = x)).Returns(commandResult);

            //Act
            await service.AddBotToGroup(criteria);

            //Assert
            await baseService.Received().ExecuteNonQueryAsync(Arg.Any<AddBotToGroupCommand>());
        }

        [Fact]
        public async Task AddBotToGroup_IsSuccessful_ReturnExpectedCommandResult()
        {
            var criteria = new AddBotToGroupCommand
            {
                BotConversationId = "aa",
                ConversationId = "bb"
            };
            var commandResult = new CommandResult { Code = 0 };
            var service = GetService();

            baseService.ExecuteNonQueryAsync(Arg.Do<AddBotToGroupCommand>(x => criteria = x)).Returns(commandResult);

            //Act
            var actual = await service.AddBotToGroup(criteria);

            //Assert
            Assert.Equal(commandResult, actual);
        }

        [Fact]
        public async Task RemoveBotOutGroup_WhenCalled_ExpectedCallExecuteNonQueryAsync()
        {
            var criteria = new RemoveBotOutGroupCommand
            {
                BotConversationId = "aa",
                ConversationId = "bb"
            };
            var commandResult = new CommandResult { Code = 0 };
            var service = GetService();

            baseService.ExecuteNonQueryAsync(Arg.Do<RemoveBotOutGroupCommand>(x => criteria = x)).Returns(commandResult);

            //Act
            await service.RemoveBotOutGroup(criteria);

            //Assert
            await baseService.Received().ExecuteNonQueryAsync(Arg.Any<RemoveBotOutGroupCommand>());
        }

        [Fact]
        public async Task RemoveBotOutGroup_IsSuccessful_ReturnExpectedCommandResult()
        {
            var criteria = new RemoveBotOutGroupCommand
            {
                BotConversationId = "aa",
                ConversationId = "bb"
            };
            var commandResult = new CommandResult { Code = 0 };
            var service = GetService();

            baseService.ExecuteNonQueryAsync(Arg.Do<RemoveBotOutGroupCommand>(x => criteria = x)).Returns(commandResult);

            //Act
            var actual = await service.RemoveBotOutGroup(criteria);

            //Assert
            Assert.Equal(commandResult, actual);
        }

        [Fact]
        public async Task UpdateGroupTopic_WhenCalled_ExpectedCallExecuteNonQueryAsync()
        {
            var criteria = new UpdateGroupTopicCommand
            {
                ConversationId = "aa",
                GroupName = "bb"
            };
            var commandResult = new CommandResult { Code = 0 };
            var service = GetService();

            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateGroupTopicCommand>(x => criteria = x)).Returns(commandResult);

            //Act
            await service.UpdateGroupTopic(criteria);

            //Assert
            await baseService.Received().ExecuteNonQueryAsync(Arg.Any<UpdateGroupTopicCommand>());
        }

        [Fact]
        public async Task UpdateGroupTopic_IsSuccessful_ReturnExpectedCommandResult()
        {
            var criteria = new UpdateGroupTopicCommand
            {
                ConversationId = "aa",
                GroupName = "bb"
            };
            var commandResult = new CommandResult { Code = 0 };
            var service = GetService();

            baseService.ExecuteNonQueryAsync(Arg.Do<UpdateGroupTopicCommand>(x => criteria = x)).Returns(commandResult);

            //Act
            var actual = await service.UpdateGroupTopic(criteria);

            //Assert
            Assert.Equal(commandResult, actual);
        }

        [Fact]
        public async Task MigratedToSupergroupAsync_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var command = new MigratedToSupergroupCommand();
            var commandResult = new CommandResult { Code = 0 };
            var service = GetService();
            baseService.ExecuteNonQueryAsync(command).Returns(commandResult);

            // Act
            var actual = await service.MigratedToSupergroupAsync(command);

            // Assert
            Assert.Equal(commandResult, actual);
        }

        [Fact]
        public async Task AddMessageLog_WhenCalled_ExpectedCallExecuteNonQueryAsync()
        {
            var criteria = new AddMessageLogCommand();
            var commandResult = new CommandResult { Code = 0 };
            var service = GetService();

            baseService.ExecuteNonQueryAsync(Arg.Do<AddMessageLogCommand>(x => criteria = x)).Returns(commandResult);

            //Act
            await service.AddMessageLog(criteria);

            //Assert
            await baseService.Received().ExecuteNonQueryAsync(Arg.Any<AddMessageLogCommand>());
        }

        [Fact]
        public async Task AddMessageLog_IsSuccessful_ReturnExpectedCommandResult()
        {
            var criteria = new AddMessageLogCommand();
            var commandResult = new CommandResult { Code = 0 };
            var service = GetService();

            baseService.ExecuteNonQueryAsync(Arg.Do<AddMessageLogCommand>(x => criteria = x)).Returns(commandResult);

            //Act
            var actual = await service.AddMessageLog(criteria);

            //Assert
            Assert.Equal(commandResult, actual);
        }

        [Fact]
        public void GetDistinctNotificationConversation_WhenCalledReturnsExpectedResult()
        {
            var input = new List<NotificationMessage>
            {
                new NotificationMessage { NotificationId = 1, Recipients = new List<ConversationInfo> { new ConversationInfo {ConversationId = "A"},
                new ConversationInfo {ConversationId = "D"}} },
                new NotificationMessage { NotificationId = 2, Recipients = new List<ConversationInfo> { new ConversationInfo {ConversationId = "B"} } },
                new NotificationMessage { NotificationId = 1, Recipients = new List<ConversationInfo> { new ConversationInfo {ConversationId = "A"},
                    new ConversationInfo { ConversationId = "B" }, new ConversationInfo { ConversationId = "C" } } },
            };
            var expectedResult = new List<NotificationMessage>
            {
                new NotificationMessage { NotificationId = 1,
                    Recipients = new List<ConversationInfo> { new ConversationInfo {ConversationId = "A"}, new ConversationInfo { ConversationId = "D" },
                    new ConversationInfo { ConversationId = "B" }, new ConversationInfo { ConversationId = "C" } } },
                new NotificationMessage { NotificationId = 2, Recipients = new List<ConversationInfo> { new ConversationInfo {ConversationId = "B"} } },
            };

            var service = GetService();
            var actual = service.GetDistinctNotificationConversation(input);
            Assert.Equal(expectedResult.Count, actual.Count);
            Assert.Equal(expectedResult.FirstOrDefault(a => a.NotificationId == 1).Recipients.Count(),
                actual.FirstOrDefault(a => a.NotificationId == 1).Recipients.Count());

            Assert.Equal(expectedResult.FirstOrDefault(a => a.NotificationId == 2).Recipients.Count(),
               actual.FirstOrDefault(a => a.NotificationId == 2).Recipients.Count());
        }

        private ConversationService GetService()
        {
            baseService = Substitute.For<IBaseService>();
            settings = new CheckDuplicatedMessageLogSetting
            {
                IsEnabled = true,
                TimeSpanInMinutes = 20,
                ExceptionConversatiionIDs = new[] { "A" }
            };

            return new ConversationService(baseService, settings);
        }
    }
}