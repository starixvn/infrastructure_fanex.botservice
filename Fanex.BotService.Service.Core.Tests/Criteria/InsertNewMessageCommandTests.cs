using Fanex.BotService.Service.Core.Criteria.Message;

namespace Fanex.BotService.Core.Tests.Criteria
{
    public class InsertNewMessageCommandTests
    {
        [Fact]
        public void GetSettingKey_ShouldReturnCorrectKey()
        {
            // Arrange
            var command = new InsertNewMessageCommand();

            // Act
            var result = command.GetSettingKey();

            // Assert
            Assert.Equal("Message_Insert", result);
        }

        [Fact]
        public void IsValid_ShouldReturnTrue_WhenAllPropertiesAreValid()
        {
            // Arrange
            var command = new InsertNewMessageCommand
            {
                AppId = 1,
                ConversationId = "conversation123",
                Content = "Hello, World!",
                ChannelId = 1,
                MessageId = 123,
                Status = "Sent",
                CreatedDate = DateTime.Now,
                Attachments = "attachment1"
            };

            // Act
            var result = command.IsValid();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenAppIdIsInvalid()
        {
            // Arrange
            var command = new InsertNewMessageCommand
            {
                AppId = 0,
                ConversationId = "conversation123",
                Content = "Hello, World!",
                ChannelId = 1,
                MessageId = 123,
                Status = "Sent",
                CreatedDate = DateTime.Now,
                Attachments = "attachment1"
            };

            // Act
            var result = command.IsValid();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenConversationIdIsInvalid()
        {
            // Arrange
            var command = new InsertNewMessageCommand
            {
                AppId = 1,
                ConversationId = "",
                Content = "Hello, World!",
                ChannelId = 1,
                MessageId = 123,
                Status = "Sent",
                CreatedDate = DateTime.Now,
                Attachments = "attachment1"
            };

            // Act
            var result = command.IsValid();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenContentIsInvalid()
        {
            // Arrange
            var command = new InsertNewMessageCommand
            {
                AppId = 1,
                ConversationId = "conversation123",
                Content = "",
                ChannelId = 1,
                MessageId = 123,
                Status = "Sent",
                CreatedDate = DateTime.Now,
                Attachments = "attachment1"
            };

            // Act
            var result = command.IsValid();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenChannelIdIsInvalid()
        {
            // Arrange
            var command = new InsertNewMessageCommand
            {
                AppId = 1,
                ConversationId = "conversation123",
                Content = "Hello, World!",
                ChannelId = 0,
                MessageId = 123,
                Status = "Sent",
                CreatedDate = DateTime.Now,
                Attachments = "attachment1"
            };

            // Act
            var result = command.IsValid();

            // Assert
            Assert.False(result);
        }
    }
}