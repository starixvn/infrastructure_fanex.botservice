namespace NexGoal.Core.UnitTests
{
    using System.Collections;
    using System.Threading.Tasks;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.NCB.Shared.Models;
    using Fanex.NCB.Shared.Repositories;
    using NSubstitute;
    using Xunit;

    public class BaseRepositoryTests
    {
        private readonly IObjectDb objectDb;

        public BaseRepositoryTests()
        {
            DbSettingProviderManager
            .StartTestingSession()
            .Run();

            objectDb = Substitute.For<IObjectDb>();
            ObjectDbFactory.Mock((key, time) => objectDb);
        }

        #region ExecuteNonQueryAsync

        [Fact]
        public async Task ExecuteNonQueryAsync_CriteriaNotNull_CallExecuteNonQueryAsync()
        {
            // Arrange
            var command = FakeCommand();
            var repository = new BaseRepository();

            // Action
            var result = await repository.ExecuteNonQueryAsync(command);

            // Assert
            var expected = await objectDb.Received().ExecuteNonQueryAsync(command);

            Assert.Equal(expected, result.Code);
        }

        [Fact]
        public async Task ExecuteNonQueryAsync_OutputParams_ReturnAsResult()
        {
            // Arrange
            var repository = new BaseRepository();
            var command = FakeCommand();
            var expected = GetSuccessResult();
            var outputParams = FakeOutputParamsSuccess();
            objectDb.GetOutputParams().Returns(outputParams);

            // Action
            var result = await repository.ExecuteNonQueryAsync(command);

            // Assert
            Assert.Equal(expected.Code, result.Code);
            Assert.Equal(expected.Message, result.Message);
        }

        #endregion ExecuteNonQueryAsync

        #region Helpers

        private static INonQueryCommand FakeCommand()
        {
            var command = Substitute.For<INonQueryCommand>();

            command.GetSettingKey().Returns("key");
            command.IsValid().Returns(true);

            return command;
        }

        private static Hashtable FakeOutputParamsSuccess()
        {
            return new Hashtable
            {
                { "errcode", 0 },
                { "errmsg", string.Empty },
            };
        }

        private CommandResult GetSuccessResult()
        {
            return new CommandResult
            {
                Code = 0,
                Message = string.Empty
            };
        }

        #endregion Helpers
    }
}