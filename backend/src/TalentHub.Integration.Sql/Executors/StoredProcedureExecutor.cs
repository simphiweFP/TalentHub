using System.Data;
using Dapper;
using TalentHub.Integration.Sql.Abstractions;

namespace TalentHub.Integration.Sql.Executors;

public sealed class StoredProcedureExecutor(IDbConnectionFactory connectionFactory) : IStoredProcedureExecutor
{
    public async Task<IEnumerable<T>> QueryAsync<T>(string storedProcedure, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CancellationToken cancellationToken = default)
    {
        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(storedProcedure, parameters, transaction, commandTimeout, CommandType.StoredProcedure, cancellationToken: cancellationToken);
        return await connection.QueryAsync<T>(command).ConfigureAwait(false);
    }

    public async Task<int> ExecuteAsync(string storedProcedure, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CancellationToken cancellationToken = default)
    {
        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(storedProcedure, parameters, transaction, commandTimeout, CommandType.StoredProcedure, cancellationToken: cancellationToken);
        return await connection.ExecuteAsync(command).ConfigureAwait(false);
    }

    public async Task<T?> ExecuteScalarAsync<T>(string storedProcedure, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CancellationToken cancellationToken = default)
    {
        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(storedProcedure, parameters, transaction, commandTimeout, CommandType.StoredProcedure, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<T>(command).ConfigureAwait(false);
    }
}
