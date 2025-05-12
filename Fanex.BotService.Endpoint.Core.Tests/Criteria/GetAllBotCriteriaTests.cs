using Fanex.BotService.Core.Criteria;

namespace Fanex.BotService.Endpoint.Core.Tests.Criteria;

public class GetAllBotCriteriaTests
{
    [Fact]
    public void GetSettingKey_ShouldReturnCorrectKey()
    {
        // Arrange
        var criteria = new GetAllBotCriteria();

        // Act
        var result = criteria.GetSettingKey();

        // Assert
        Assert.Equal("Bot_GetAll", result);
    }

    [Fact]
    public void IsValid_ShouldReturnTrue()
    {
        // Arrange
        var criteria = new GetAllBotCriteria();

        // Act
        var result = criteria.IsValid();

        // Assert
        Assert.True(result);
    }
}