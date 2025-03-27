using Fanex.BotService.Core.Criteria;

namespace Fanex.BotService.Endpoint.Core.Tests.Criteria;

public class MigratedToSupergroupCommandTests
{
    [Fact]
    public void GetSettingKey_ShouldReturnCorrectKey()
    {
        // Arrange
        var command = new MigratedToSupergroupCommand();

        // Act
        var result = command.GetSettingKey();

        // Assert
        Assert.Equal("NCB_Conversation_MigratedToSupergroup", result);
    }

    [Fact]
    public void IsValid_ShouldReturnTrue_WhenFromAndToConversationIdAreNotEmpty()
    {
        // Arrange
        var command = new MigratedToSupergroupCommand
        {
            FromConversationId = "123",
            ToConversationId = "456"
        };

        // Act
        var result = command.IsValid();

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenFromConversationIdIsEmpty()
    {
        // Arrange
        var command = new MigratedToSupergroupCommand
        {
            FromConversationId = "",
            ToConversationId = "456"
        };

        // Act
        var result = command.IsValid();

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValid_ShouldReturnFalse_WhenToConversationIdIsEmpty()
    {
        // Arrange
        var command = new MigratedToSupergroupCommand
        {
            FromConversationId = "123",
            ToConversationId = ""
        };

        // Act
        var result = command.IsValid();

        // Assert
        Assert.False(result);
    }
}