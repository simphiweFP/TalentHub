using Dapper;
using System.Data;

namespace TalentHub.Integration.Sql.Abstractions;

public interface IQueryExecutor
{
    Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default);

    Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default);

    Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default);

    Task<SqlMapper.GridReader> QueryMultipleAsync(string sql, object? parameters = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancellationToken = default);
}
