namespace NexGoal.Core.UnitTests.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Fanex.Data.Repository;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Repositories;
    using Fanex.NCB.Shared.Services;
    using NSubstitute;
    using Xunit;

    public class BaseServiceTests
    {
        private IDynamicRepository dynamicRepository;
        private IBaseRepository baseRepository;

        #region GetDataAsync

        [Fact]
        public async Task GetDataAsync_CriteriaIsNull_ReturnEmptyList()
        {
            // Arrange
            var baseService = GetBaseService();

            // Act
            var actual = await baseService.GetDataAsync<int>(null);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetDataAsync_CriteriaIsInvalid_ReturnEmptyList()
        {
            // Arrange
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(false);
            var baseService = GetBaseService();

            // Act
            var actual = await baseService.GetDataAsync<FunctionSession>(criteria);

            // Assert
            Assert.Empty(actual);
        }

        [Fact]
        public async Task GetDataAsync_CriteriaIsValid_ReturnExpectedData()
        {
            // Arrange
            var expected = new List<FunctionSession> {
                new FunctionSession { FunctionId = 1, FunctionKey = "Configuration", Name = "Configuration" }
            };
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(true);
            var baseService = GetBaseService();
            dynamicRepository.FetchAsync<FunctionSession>(criteria).Returns(expected);

            // Act
            var actual = await baseService.GetDataAsync<FunctionSession>(criteria);

            // Assert
            Assert.NotEmpty(actual);
            Assert.Equal(expected, actual);
        }

        #endregion GetDataAsync

        #region GetDataEntityAsync

        [Fact]
        public async Task GetDataEntityAsync_CriteriaIsNull_ReturnDefaultValue()
        {
            // Arrange
            var baseService = GetBaseService();

            // Act
            var actual = await baseService.GetDataEntityAsync<int>(null);

            // Assert
            Assert.Equal(0, actual);
        }

        [Fact]
        public async Task GetDataEntityAsync_CriteriaIsInvalid_ReturnDefaultValue()
        {
            // Arrange
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(false);
            var baseService = GetBaseService();

            // Act
            var actual = await baseService.GetDataEntityAsync<int>(criteria);

            // Assert
            Assert.Equal(0, actual);
        }

        [Fact]
        public async Task GetDataEntityAsync_CriteriaIsValid_ReturnExpectedData()
        {
            // Arrange
            var expected = 1;
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(true);
            var baseService = GetBaseService();
            dynamicRepository.GetAsync<int>(criteria).Returns(expected);

            // Act
            var actual = await baseService.GetDataEntityAsync<int>(criteria);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion GetDataEntityAsync

        #region FetchMultipleAsync

        [Fact]
        public async Task FetchMultipleAsync_CriteriaIsNull_ReturnTupleWithEmptyList()
        {
            // Arrange
            var baseService = GetBaseService();

            // Act
            var actual = await baseService.FetchMultipleAsync<int, int>(null);

            // Assert
            Assert.Empty(actual.Item1);
            Assert.Empty(actual.Item2);
        }

        [Fact]
        public async Task FetchMultipleAsync_CriteriaIsInvalid_ReturnTupleWithEmptyList()
        {
            // Arrange
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(false);
            var baseService = GetBaseService();

            // Act
            var actual = await baseService.FetchMultipleAsync<int, int>(criteria);

            // Assert
            Assert.Empty(actual.Item1);
            Assert.Empty(actual.Item2);
        }

        [Fact]
        public async Task FetchMultipleAsync_CriteriaIsValid_ReturnExpectedData()
        {
            // Arrange
            var expected = new ValueTuple<IEnumerable<int>, IEnumerable<int>>(new List<int> { 1, 2 }, new List<int> { 1, 2 });
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(true);
            var baseService = GetBaseService();
            dynamicRepository.FetchMultipleAsync<int, int>(criteria).Returns(expected);

            // Act
            var actual = await baseService.FetchMultipleAsync<int, int>(criteria);

            // Assert
            Assert.Equal(expected, actual);
        }

        [Fact]
        public async Task FetchMultipleAsyncThird_CriteriaIsNull_ReturnTupleWithEmptyList()
        {
            // Arrange
            var baseService = GetBaseService();

            // Act
            var actual = await baseService.FetchMultipleAsync<int, int, int>(null);

            // Assert
            Assert.Empty(actual.Item1);
            Assert.Empty(actual.Item2);
            Assert.Empty(actual.Item3);
        }

        [Fact]
        public async Task FetchMultipleAsyncThird_CriteriaIsInvalid_ReturnTupleWithEmptyList()
        {
            // Arrange
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(false);
            var baseService = GetBaseService();

            // Act
            var actual = await baseService.FetchMultipleAsync<int, int, int>(criteria);

            // Assert
            Assert.Empty(actual.Item1);
            Assert.Empty(actual.Item2);
            Assert.Empty(actual.Item3);
        }

        [Fact]
        public async Task FetchMultipleAsyncThird_CriteriaIsValid_ReturnExpectedData()
        {
            // Arrange
            var expected = new ValueTuple<IEnumerable<int>, IEnumerable<int>, IEnumerable<int>>(new List<int> { 1, 2 }, new List<int> { 1, 2 }, new List<int> { 1, 2 });
            var criteria = Substitute.For<ICriteria>();
            criteria.IsValid().Returns(true);
            var baseService = GetBaseService();
            dynamicRepository.FetchMultipleAsync<int, int, int>(criteria).Returns(expected);

            // Act
            var actual = await baseService.FetchMultipleAsync<int, int, int>(criteria);

            // Assert
            Assert.Equal(expected, actual);
        }

        #endregion FetchMultipleAsync

        #region ExecuteNonQueryAsync

        [Fact]
        public async Task ExecuteNonQueryAsync_CriteriaIsNull_ReturnDefaultCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var baseService = GetBaseService();

            // Act
            var actual = await baseService.ExecuteNonQueryAsync(null);

            // Assert
            Assert.Equal(expected.Code, actual.Code);
            Assert.Equal(expected.IsSuccessful, actual.IsSuccessful);
            Assert.Equal(expected.Message, actual.Message);
        }

        [Fact]
        public async Task ExecuteNonQueryAsync_CriteriaIsInvalid_ReturnDefaultCommandResult()
        {
            // Arrange
            var expected = new CommandResult();
            var command = Substitute.For<INonQueryCommand>();
            command.IsValid().Returns(false);
            var baseService = GetBaseService();

            // Act
            var actual = await baseService.ExecuteNonQueryAsync(command);

            // Assert
            Assert.Equal(expected.Code, actual.Code);
            Assert.Equal(expected.IsSuccessful, actual.IsSuccessful);
            Assert.Equal(expected.Message, actual.Message);
        }

        [Fact]
        public async Task ExecuteNonQueryAsync_CriteriaIsValid_ExpectedExecuteQuery()
        {
            // Arrange
            var command = Substitute.For<INonQueryCommand>();
            command.IsValid().Returns(true);
            var baseService = GetBaseService();
            // Act
            await baseService.ExecuteNonQueryAsync(command);

            // Assert
            await baseRepository.Received().ExecuteNonQueryAsync(Arg.Is<INonQueryCommand>(x => x.IsValid()));
        }

        #endregion ExecuteNonQueryAsync

        #region Helpers

        private static IDynamicRepository FakeDynamicRepository()
        {
            var repository = Substitute.For<IDynamicRepository>();

            return repository;
        }

        private static IBaseRepository FakeBaseRepository()
        {
            var repository = Substitute.For<IBaseRepository>();

            return repository;
        }

        private BaseService GetBaseService()
        {
            dynamicRepository = FakeDynamicRepository();
            baseRepository = FakeBaseRepository();
            var service = new BaseService(dynamicRepository, baseRepository);

            return service;
        }

        #endregion Helpers
    }

    public class FunctionSession
    {
        public int FunctionId { get; set; }
        public string FunctionKey { get; set; }
        public string Name { get; set; }
    }
}