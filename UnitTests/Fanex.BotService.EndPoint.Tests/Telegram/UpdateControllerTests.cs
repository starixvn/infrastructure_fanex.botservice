namespace Fanex.NCB.BotService.EndPoint.Tests.Telegram
{
    using System.Threading.Tasks;
    using AutoFixture;
    using Fanex.Logging;
    using Fanex.NCB.BotService.Core.Criteria;
    using Fanex.NCB.BotService.Core.Services;
    using Fanex.NCB.BotService.Core.Services.Interfaces;
    using Fanex.NCB.BotService.EndPoint.Tests.TestableClasses.Telegram;
    using Fanex.NCB.Shared.Models;
    using global::Telegram.Bot.Types;
    using Microsoft.AspNetCore.Mvc;
    using NSubstitute;
    using Xunit;

    public class UpdateControllerTests
    {
        private readonly IFixture fixture = new Fixture();
        private ILogger logger;
        private ITelegramBotService botService;
        private IConversationService conversationService;

        #region Update

        [Fact]
        public async Task Post_UpdateParamIsNull_DoNothing()
        {
            // Arrange
            Update update = null;
            var controller = GetController();

            // Act
            var result = await controller.Post(1, update);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            await botService.DidNotReceive().GetBotInfoAsync(1);
        }

        [Fact]
        public async Task Post_UpdateTypeNotEqualMessage_DoNothing()
        {
            // Arrange
            var update = new Update();
            var controller = GetController();

            // Act
            var result = await controller.Post(1, update);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            await botService.DidNotReceive().GetBotInfoAsync(1);
        }

        [Fact]
        public async Task Post_BotIsNotExist_ExpectedWriteLog()
        {
            // Arrange
            User bot = null;
            var update = new Update { Message = new Message() };
            var controller = GetController();
            botService.GetBotInfoAsync(1).Returns(bot);

            // Act
            var result = await controller.Post(1, update);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal("Telegram Bot 1 is invalid.", okResult.Value);

            await logger.Received().ErrorAsync("Telegram Bot 1 is invalid.");
        }

        [Fact]
        public async Task Post_ChatTitleChanged_CallCorrectlyMethod()
        {
            // Arrange
            var bot = fixture.Create<User>();
            var update = new Update
            {
                Message = new Message
                {
                    NewChatTitle = "chat title"
                }
            };
            var controller = GetController();
            botService.GetBotInfoAsync(1).Returns(bot);
            controller.Protected("EditChatTitledAsync", update.Message).Returns(Task.FromResult(new CommandResult()));

            // Act
            var result = await controller.Post(1, update);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            controller.Received().Protected("EditChatTitledAsync", update.Message);
        }

        [Fact]
        public async Task Post_GroupCreated_CallCorrectlyMethod()
        {
            // Arrange
            var bot = fixture.Create<User>();
            var update = new Update
            {
                Message = new Message
                {
                    GroupChatCreated = true
                }
            };
            var controller = GetController();
            botService.GetBotInfoAsync(1).Returns(bot);
            controller.Protected("AddBotToGroupAsync", update.Message, bot).Returns(Task.FromResult(new CommandResult()));

            // Act
            var result = await controller.Post(1, update);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            controller.Received().Protected("AddBotToGroupAsync", update.Message, bot);
        }

        [Fact]
        public async Task Post_ChatMembersAdded_CallCorrectlyMethod()
        {
            // Arrange
            var bot = fixture.Create<User>();
            var update = new Update
            {
                Message = new Message
                {
                    NewChatMembers = new User[]
                    {
                        new User { Username = "tim.trinh", IsBot = false },
                        new User { Username = "bot1", IsBot = true },
                        new User { Username = "bot2", IsBot = true }
                    }
                }
            };
            var controller = GetController();
            botService.GetBotInfoAsync(1).Returns(bot);
            controller.Protected("AddBotToGroupAsync", update.Message, Arg.Any<User>()).Returns(Task.FromResult(new CommandResult()));

            // Act
            var result = await controller.Post(1, update);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            controller.Received(2).Protected("AddBotToGroupAsync", update.Message, Arg.Any<User>());
        }

        [Fact]
        public async Task Post_ChatMemberLeft_CallCorrectlyMethod()
        {
            // Arrange
            var bot = fixture.Create<User>();
            var update = new Update
            {
                Message = new Message
                {
                    LeftChatMember = new User { Username = "bot" }
                }
            };
            var controller = GetController();
            botService.GetBotInfoAsync(1).Returns(bot);
            controller.Protected("RemoveBotToGroupAsync", update.Message).Returns(Task.FromResult(new CommandResult()));

            // Act
            var result = await controller.Post(1, update);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);

            controller.Received().Protected("RemoveBotToGroupAsync", update.Message);
        }

        #endregion Update

        [Fact]
        public async Task EditChatTitledAsync_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var commandResult = fixture.Create<CommandResult>();
            var message = new Message
            {
                Chat = new Chat { Id = 1 },
                NewChatTitle = "new chat title",
                From = new User { Id = 1 }
            };
            var command = new UpdateGroupTopicCommand();
            var controller = GetController();
            conversationService.UpdateGroupTopic(Arg.Do<UpdateGroupTopicCommand>(x => command = x)).Returns(commandResult);

            // Act
            var actual = await controller.Public_EditChatTitledAsync(message);

            // Assert
            Assert.Equal(commandResult, actual);

            Assert.Equal(message.Chat.Id.ToString(), command.ConversationId);
            Assert.Equal(message.NewChatTitle, command.GroupName);
            Assert.Equal(message.From.Id.ToString(), command.UpdatedBy);
        }

        [Fact]
        public async Task AddBotToGroupAsync_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var commandResult = fixture.Create<CommandResult>();
            var message = new Message
            {
                Chat = new Chat { Id = 1, Title = "chat title" },
                From = new User { Id = 1 }
            };
            var bot = fixture.Create<User>();
            var command = new AddBotToGroupCommand();
            var controller = GetController();
            conversationService.AddBotToGroup(Arg.Do<AddBotToGroupCommand>(x => command = x)).Returns(commandResult);

            // Act
            var actual = await controller.Public_AddBotToGroupAsync(message, bot);

            // Assert
            Assert.Equal(commandResult, actual);

            Assert.Equal(bot.Id.ToString(), command.BotConversationId);
            Assert.Equal(bot.FirstName, command.BotName);
            Assert.Equal(message.Chat.Id.ToString(), command.ConversationId);
            Assert.Equal(message.Chat.Title, command.GroupName);
            Assert.Equal(2, command.ChannelId);
            Assert.Empty(command.ServiceUrl);
            Assert.Equal(message.From.Id.ToString(), command.UpdatedBy);
        }

        [Fact]
        public async Task RemoveBotToGroupAsync_IsNormalUser_ReturnExpectedCommandResult()
        {
            // Arrange
            var message = new Message
            {
                Chat = new Chat { Id = 1 },
                LeftChatMember = new User { Id = 2 },
                From = new User { Id = 1 }
            };
            var controller = GetController();

            // Act
            var actual = await controller.Public_RemoveBotToGroupAsync(message);

            // Assert
            Assert.True(actual.IsSuccessful);
            Assert.Empty(actual.Message);

            await conversationService.DidNotReceive().RemoveBotOutGroup(Arg.Any<RemoveBotOutGroupCommand>());
        }

        [Fact]
        public async Task RemoveBotToGroupAsync_IsBot_ReturnExpectedCommandResult()
        {
            // Arrange
            var commandResult = fixture.Create<CommandResult>();
            var message = new Message
            {
                Chat = new Chat { Id = 1 },
                LeftChatMember = new User { Id = 2, IsBot = true },
                From = new User { Id = 1 }
            };
            var command = new RemoveBotOutGroupCommand();
            var controller = GetController();
            conversationService.RemoveBotOutGroup(Arg.Do<RemoveBotOutGroupCommand>(x => command = x)).Returns(commandResult);

            // Act
            var actual = await controller.Public_RemoveBotToGroupAsync(message);

            // Assert
            Assert.Equal(commandResult, actual);

            Assert.Equal(message.Chat.Id.ToString(), command.ConversationId);
            Assert.Equal(message.LeftChatMember.Id.ToString(), command.BotConversationId);
            Assert.Equal(message.From.Id.ToString(), command.UpdatedBy);
        }

        [Fact]
        public async Task MigratedToSupergroupAsync_WhenCalled_ReturnExpectedCommandResult()
        {
            // Arrange
            var commandResult = fixture.Create<CommandResult>();
            var message = new Message
            {
                Chat = new Chat { Id = 1 },
                From = new User { Id = 1 },
                MigrateToChatId = 2
            };
            var command = new MigratedToSupergroupCommand();
            var controller = GetController();
            conversationService.MigratedToSupergroupAsync(Arg.Do<MigratedToSupergroupCommand>(x => command = x)).Returns(commandResult);

            // Act
            var actual = await controller.Public_MigratedToSupergroupAsync(message);

            // Assert
            Assert.Equal(commandResult, actual);

            Assert.Equal(message.Chat.Id.ToString(), command.FromConversationId);
            Assert.Equal(message.MigrateToChatId.ToString(), command.ToConversationId);
            Assert.Equal(message.From.Id.ToString(), command.UpdatedBy);
        }

        #region Helpers

        private UpdateControllerTestable GetController()
        {
            logger = Substitute.For<ILogger>();
            botService = Substitute.For<ITelegramBotService>();
            conversationService = Substitute.For<IConversationService>();
            return Substitute.ForPartsOf<UpdateControllerTestable>(logger, botService, conversationService);
        }

        #endregion Helpers
    }
}