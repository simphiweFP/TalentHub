using System.Data;
using Dapper;
using TalentHub.Integration.Sql.Abstractions;

namespace TalentHub.Integration.Sql.Executors;

public sealed class QueryExecutor(IDbConnectionFactory connectionFactory) : IQueryExecutor
{
    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
    {
        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(sql, parameters, transaction, commandTimeout, commandType, cancellationToken: cancellationToken);
        return await connection.QueryAsync<T>(command).ConfigureAwait(false);
    }

    public async Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
    {
        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(sql, parameters, transaction, commandTimeout, commandType, cancellationToken: cancellationToken);
        return await connection.QuerySingleOrDefaultAsync<T>(command).ConfigureAwait(false);
    }

    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
    {
        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(sql, parameters, transaction, commandTimeout, commandType, cancellationToken: cancellationToken);
        return await connection.QueryFirstOrDefaultAsync<T>(command).ConfigureAwait(false);
    }

    public async Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
    {
        var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(sql, parameters, transaction, commandTimeout, commandType, cancellationToken: cancellationToken);
        return await connection.QueryMultipleAsync(command).ConfigureAwait(false);
    }
}
