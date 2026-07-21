using System.Data;
using Dapper;
using TalentHub.Integration.Sql.Abstractions;

namespace TalentHub.Integration.Sql.Executors;

public sealed class CommandExecutor(IDbConnectionFactory connectionFactory) : ICommandExecutor
{
    public async Task<int> ExecuteAsync(string sql, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
    {
        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(sql, parameters, transaction, commandTimeout, commandType, cancellationToken: cancellationToken);
        return await connection.ExecuteAsync(command).ConfigureAwait(false);
    }

    public async Task<T?> ExecuteScalarAsync<T>(string sql, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default)
    {
        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        var command = new CommandDefinition(sql, parameters, transaction, commandTimeout, commandType, cancellationToken: cancellationToken);
        return await connection.ExecuteScalarAsync<T>(command).ConfigureAwait(false);
    }
}
