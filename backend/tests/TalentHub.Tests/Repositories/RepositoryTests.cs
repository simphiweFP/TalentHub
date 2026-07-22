using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TalentHub.Integration.Sql.Abstractions;
using TalentHub.Integration.Sql.Repositories;

namespace TalentHub.Tests.Repositories;

public sealed class RepositoryTests
{
    [Fact]
    public async Task BaseRepository_delegates_query_command_and_stored_procedure_calls()
    {
        // Arrange
        var queryExecutor = new RecordingQueryExecutor();
        var commandExecutor = new RecordingCommandExecutor();
        var storedProcedureExecutor = new RecordingStoredProcedureExecutor();
        var repository = new TestRepository(
            new ThrowingConnectionFactory(),
            queryExecutor,
            commandExecutor,
            storedProcedureExecutor,
            new NoOpTransactionManager());

        // Act
        var queryResult = await repository.RunQueryAsync().ConfigureAwait(false);
        var commandResult = await repository.RunCommandAsync().ConfigureAwait(false);
        var storedProcedureResult = await repository.RunStoredProcedureAsync().ConfigureAwait(false);

        // Assert
        Assert.Equal(2, queryResult.Count());
        Assert.Equal(7, commandResult);
        Assert.Single(storedProcedureResult);
        Assert.Equal("SELECT 1", queryExecutor.LastSql);
        Assert.Equal("UPDATE Jobs SET IsActive = 1", commandExecutor.LastSql);
        Assert.Equal("usp_SearchJobs", storedProcedureExecutor.LastStoredProcedure);
    }

    private sealed class TestRepository(
        IDbConnectionFactory connectionFactory,
        IQueryExecutor queryExecutor,
        ICommandExecutor commandExecutor,
        IStoredProcedureExecutor storedProcedureExecutor,
        ITransactionManager transactionManager)
        : BaseRepository<object>(connectionFactory, queryExecutor, commandExecutor, storedProcedureExecutor, transactionManager)
    {
        public Task<IEnumerable<int>> RunQueryAsync() => QueryAsync<int>("SELECT 1");

        public Task<int> RunCommandAsync() => ExecuteAsync("UPDATE Jobs SET IsActive = 1");

        public Task<IEnumerable<string>> RunStoredProcedureAsync() => ExecuteStoredProcedureAsync<string>("usp_SearchJobs");
    }

    private sealed class RecordingQueryExecutor : IQueryExecutor
    {
        public string? LastSql { get; private set; }

        public Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            LastSql = sql;
            return Task.FromResult<IEnumerable<T>>([ (T)(object)1!, (T)(object)2! ]);
        }

        public Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
            => Task.FromResult<T?>(default);

        public Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
            => Task.FromResult<T?>(default);

        public Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();
    }

    private sealed class RecordingCommandExecutor : ICommandExecutor
    {
        public string? LastSql { get; private set; }

        public Task<int> ExecuteAsync(string sql, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
        {
            LastSql = sql;
            return Task.FromResult(7);
        }

        public Task<T?> ExecuteScalarAsync<T>(string sql, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
            => Task.FromResult<T?>(default);
    }

    private sealed class RecordingStoredProcedureExecutor : IStoredProcedureExecutor
    {
        public string? LastStoredProcedure { get; private set; }

        public Task<IEnumerable<T>> QueryAsync<T>(string storedProcedure, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CancellationToken cancellationToken = default)
        {
            LastStoredProcedure = storedProcedure;
            return Task.FromResult<IEnumerable<T>>([ (T)(object)"ok"! ]);
        }

        public Task<int> ExecuteAsync(string storedProcedure, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CancellationToken cancellationToken = default)
            => Task.FromResult(1);

        public Task<T?> ExecuteScalarAsync<T>(string storedProcedure, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CancellationToken cancellationToken = default)
            => Task.FromResult<T?>(default);
    }

    private sealed class ThrowingConnectionFactory : IDbConnectionFactory
    {
        public SqlConnection CreateConnection() => throw new NotSupportedException();

        public Task<SqlConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
            => throw new NotSupportedException();
    }

    private sealed class NoOpTransactionManager : ITransactionManager
    {
        public Task ExecuteAsync(Func<IDbTransaction, CancellationToken, Task> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
            => Task.CompletedTask;

        public Task<TResult> ExecuteAsync<TResult>(Func<IDbTransaction, CancellationToken, Task<TResult>> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
            => Task.FromResult(default(TResult)!);
    }
}
