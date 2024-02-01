namespace Fanex.NCB.BotService.Core.Tests.Criteria
{
    using Fanex.NCB.BotService.Core.Criteria;
    using Xunit;

    public class MigratedToSupergroupCommandTests
    {
        [Fact]
        public void GetSettingKey_WhenCalled_ReturnExpectedSettingKey()
        {
            // Arrange
            var command = new MigratedToSupergroupCommand();
            var expected = "NCB_Conversation_MigratedToSupergroup";

            // Act
            var actual = command.GetSettingKey();

            // Assert
            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData("", "1")]
        [InlineData("1", "")]
        public void IsValid_ParamsAreInvalid_ReturnFalse(string fromConversationId, string toConversationId)
        {
            // Arrange
            var command = new MigratedToSupergroupCommand
            {
                FromConversationId = fromConversationId,
                ToConversationId = toConversationId
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
            var command = new MigratedToSupergroupCommand
            {
                FromConversationId = "FromConversationId",
                ToConversationId = "ToConversationId"
            };

            // Act
            var actual = command.IsValid();

            // Assert
            Assert.True(actual);
        }
    }
}