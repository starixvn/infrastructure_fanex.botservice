using Fanex.BotService.Core.Criteria;

namespace Fanex.BotService.Endpoint.Core.Tests.Criteria;

public class RemoveBotContactCommandTests
{
    [Fact]
    public void GetSettingKey_ShouldReturnCorrectKey()
    {
        // Arrange
        var command = new RemoveBotContactCommand();

        // Act
        var result = command.GetSettingKey();

        // Assert
        Assert.Equal("Conversation_RemoveContact", result);
    }

    [Fact]
    public void IsValid_ShouldReturnTrue_WhenPropertiesAreNotEmpty()
    {
        // Arrange
        var command = new RemoveBotContactCommand
        {
            BotConversationId = "bot123",
            ConversationId = "conv123"
        };

        // Act
        var result = command.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenBotConversationIdIsEmpty()
    {
        // Arrange
        var command = new RemoveBotContactCommand
        {
            BotConversationId = "",
            ConversationId = "conv123"
        };

        // Act
        var result = command.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenConversationIdIsEmpty()
    {
        // Arrange
        var command = new RemoveBotContactCommand
        {
            BotConversationId = "bot123",
            ConversationId = ""
        };

        // Act
        var result = command.IsValid();

        // Assert
        Assert.False(result);
    }
}