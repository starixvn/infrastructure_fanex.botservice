using Fanex.BotService.Shared.Models;
using Fanex.BotService.Shared.Repositories;
using Fanex.BotService.Shared.Services;
using Fanex.Data.Repository;
using Newtonsoft.Json;
using NSubstitute;

namespace Fanex.BotService.Shared.Tests.Services;

public class BaseServiceTests
{
    private readonly IDynamicRepository _dynamicRepository;
    private readonly IBaseRepository _baseRepository;
    private readonly BaseService _baseService;

    public BaseServiceTests()
    {
        _dynamicRepository = Substitute.For<IDynamicRepository>();
        _baseRepository = Substitute.For<IBaseRepository>();
        _baseService = new BaseService(_dynamicRepository, _baseRepository);
    }

    [Fact]
    public async Task GetDataAsync_ShouldReturnData_WhenCriteriaIsValid()
    {
        // Arrange
        var criteria = Substitute.For<ICriteria>();
        criteria.IsValid().Returns(true);
        var expectedData = new List<string> { "data1", "data2" };
        _dynamicRepository.FetchAsync<string>(criteria).Returns(expectedData);

        // Act
        var result = await _baseService.GetDataAsync<string>(criteria);

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
        var result = await _baseService.GetDataAsync<string>(criteria);

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
        _baseRepository.ExecuteNonQueryAsync(command).Returns(expectedResult);

        // Act
        var result = await _baseService.ExecuteNonQueryAsync(command);

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
        var result = await _baseService.ExecuteNonQueryAsync(command);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(JsonConvert.SerializeObject(new CommandResult()), JsonConvert.SerializeObject(result));
    }
}