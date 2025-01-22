using Fanex.BotService.Shared.Models;
using Fanex.BotService.Shared.Repositories;
using Fanex.BotService.Shared.Services;
using Fanex.Data.Repository;
using Newtonsoft.Json;
using NSubstitute;

namespace Fanex.BotService.Shared.Tests.Services;

public class BaseServiceTests
{
    private readonly IDynamicRepository dynamicRepository;
    private readonly IBaseRepository baseRepository;
    private readonly BaseService baseService;

    public BaseServiceTests()
    {
        dynamicRepository = Substitute.For<IDynamicRepository>();
        baseRepository = Substitute.For<IBaseRepository>();
        baseService = new BaseService(dynamicRepository, baseRepository);
    }

    [Fact]
    public async Task GetDataAsync_ShouldReturnData_WhenCriteriaIsValid()
    {
        // Arrange
        var criteria = Substitute.For<ICriteria>();
        criteria.IsValid().Returns(true);
        var expectedData = new List<string> { "data1", "data2" };
        dynamicRepository.FetchAsync<string>(criteria).Returns(expectedData);

        // Act
        var result = await baseService.GetDataAsync<string>(criteria);

        // Assert
        Assert.Equal(expectedData, result);
    }

    [Fact]
    public async Task GetDataAsync_ShouldReturnEmptyList_WhenCriteriaIsInvalid()
    {
        // Arrange
        var criteria = Substitute.For<ICriteria>();
        criteria.IsValid().Returns(false);

        // Act
        var result = await baseService.GetDataAsync<string>(criteria);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task ExecuteNonQueryAsync_ShouldReturnCommandResult_WhenCommandIsValid()
    {
        // Arrange
        var command = Substitute.For<INonQueryCommand>();
        command.IsValid().Returns(true);
        var expectedResult = new CommandResult { Code = 0, Message = "Success" };
        baseRepository.ExecuteNonQueryAsync(command).Returns(expectedResult);

        // Act
        var result = await baseService.ExecuteNonQueryAsync(command);

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task ExecuteNonQueryAsync_ShouldReturnDefaultCommandResult_WhenCommandIsInvalid()
    {
        // Arrange
        var command = Substitute.For<INonQueryCommand>();
        command.IsValid().Returns(false);

        // Act
        var result = await baseService.ExecuteNonQueryAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(JsonConvert.SerializeObject(new CommandResult()), JsonConvert.SerializeObject(result));
    }
}