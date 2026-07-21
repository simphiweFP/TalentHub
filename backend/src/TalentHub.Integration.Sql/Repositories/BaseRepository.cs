using TalentHub.Integration.Sql.Abstractions;

namespace TalentHub.Integration.Sql.Repositories;

public abstract class BaseRepository<TEntity> : IReadRepository<TEntity>, IWriteRepository<TEntity>
{
    protected BaseRepository(IDbConnectionFactory connectionFactory, IQueryExecutor queryExecutor, ICommandExecutor commandExecutor, IStoredProcedureExecutor storedProcedureExecutor, ITransactionManager transactionManager)
    {
        ConnectionFactory = connectionFactory;
        QueryExecutor = queryExecutor;
        CommandExecutor = commandExecutor;
        StoredProcedureExecutor = storedProcedureExecutor;
        TransactionManager = transactionManager;
    }

    protected IDbConnectionFactory ConnectionFactory { get; }

    protected IQueryExecutor QueryExecutor { get; }

    protected ICommandExecutor CommandExecutor { get; }

    protected IStoredProcedureExecutor StoredProcedureExecutor { get; }

    protected ITransactionManager TransactionManager { get; }

    protected Task<IEnumerable<T>> QueryAsync<T>(string sql, object? parameters = null, CancellationToken cancellationToken = default)
        => QueryExecutor.QueryAsync<T>(sql, parameters, cancellationToken: cancellationToken);

    protected Task<T?> QuerySingleOrDefaultAsync<T>(string sql, object? parameters = null, CancellationToken cancellationToken = default)
        => QueryExecutor.QuerySingleOrDefaultAsync<T>(sql, parameters, cancellationToken: cancellationToken);

    protected Task<T?> QueryFirstOrDefaultAsync<T>(string sql, object? parameters = null, CancellationToken cancellationToken = default)
        => QueryExecutor.QueryFirstOrDefaultAsync<T>(sql, parameters, cancellationToken: cancellationToken);

    protected Task<int> ExecuteAsync(string sql, object? parameters = null, CancellationToken cancellationToken = default)
        => CommandExecutor.ExecuteAsync(sql, parameters, cancellationToken: cancellationToken);

    protected Task<T?> ExecuteScalarAsync<T>(string sql, object? parameters = null, CancellationToken cancellationToken = default)
        => CommandExecutor.ExecuteScalarAsync<T>(sql, parameters, cancellationToken: cancellationToken);

    protected Task<IEnumerable<T>> ExecuteStoredProcedureAsync<T>(string storedProcedure, object? parameters = null, CancellationToken cancellationToken = default)
        => StoredProcedureExecutor.QueryAsync<T>(storedProcedure, parameters, cancellationToken: cancellationToken);
}
