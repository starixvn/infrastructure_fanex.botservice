namespace Fanex.NCB.BotService.Core.Tests.Criteria
{
    using Fanex.NCB.BotService.Core.Criteria;
    using Xunit;

    public class AddMessageLogCommandTests
    {
        [Fact]
        public void GetSettingKey_ReturnCorrectSettingKey()
        {
            // Arrange
            var command = new AddMessageLogCommand {
                FromConversationId = string.Empty,
                ToConversationId = string.Empty,
                Content = string.Empty,
                IsSuccessfull = false,
                ErrorMessage = string.Empty
            };
            var expected = "NCB_Conversation_AddMessageLog";

            // Act
            var actual = command.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void IsValid_FromConversationIdIsEmpty_ReturnFalse()
        {
            // Arrange
            var command = new AddMessageLogCommand
            {
                FromConversationId = string.Empty,
                ToConversationId = "bb"
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ToConversationIdIsEmpty_ReturnFalse()
        {
            // Arrange
            var command = new AddMessageLogCommand
            {
                FromConversationId = "aa",
                ToConversationId = string.Empty
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.False(actual);
        }

        [Fact]
        public void IsValid_ParamsAreValid_ReturnTrue()
        {
            // Arrange
            var command = new AddMessageLogCommand
            {
                FromConversationId = "aa",
                ToConversationId = "bb"
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}
