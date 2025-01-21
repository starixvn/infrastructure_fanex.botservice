using Fanex.BotService.Core.Criteria;

namespace Fanex.BotService.Endpoint.Core.Tests.Criteria
{
    public class AddMessageLogCommandTests
    {
        [Fact]
        public void GetSettingKey_ShouldReturnCorrectKey()
        {
            // Arrange
            var command = new AddMessageLogCommand();

            // Act
            var result = command.GetSettingKey();

            // Assert
            Assert.Equal("MessageLog_Add", result);
        }

        [Fact]
        public void IsValid_ShouldReturnTrue_WhenConversationIdIsNotNullOrWhiteSpace()
        {
            // Arrange
            var command = new AddMessageLogCommand
            {
                ConversationId = "validConversationId"
            };

            // Act
            var result = command.IsValid();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValid_ShouldReturnFalse_WhenConversationIdIsNullOrWhiteSpace()
        {
            // Arrange
            var command = new AddMessageLogCommand
            {
                ConversationId = " "
            };

            // Act
            var result = command.IsValid();

            // Assert
            Assert.False(result);
        }
    }
}