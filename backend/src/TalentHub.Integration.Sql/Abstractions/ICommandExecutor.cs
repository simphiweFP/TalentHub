using System.Data;

namespace TalentHub.Integration.Sql.Abstractions;

public interface ICommandExecutor
{
    Task<int> ExecuteAsync(string sql, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default);

    Task<T?> ExecuteScalarAsync<T>(string sql, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default);
}
