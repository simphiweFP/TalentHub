using System.Data;

namespace TalentHub.Integration.Sql.Abstractions;

public interface ITransactionManager
{
    Task ExecuteAsync(Func<IDbTransaction, CancellationToken, Task> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);

    Task<TResult> ExecuteAsync<TResult>(Func<IDbTransaction, CancellationToken, Task<TResult>> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default);
}
