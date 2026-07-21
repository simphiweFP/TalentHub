using System.Data;

namespace TalentHub.Integration.Sql.Abstractions;

public interface IStoredProcedureExecutor
{
    Task<IEnumerable<T>> QueryAsync<T>(string storedProcedure, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CancellationToken cancellationToken = default);

    Task<int> ExecuteAsync(string storedProcedure, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CancellationToken cancellationToken = default);

    Task<T?> ExecuteScalarAsync<T>(string storedProcedure, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CancellationToken cancellationToken = default);
}
