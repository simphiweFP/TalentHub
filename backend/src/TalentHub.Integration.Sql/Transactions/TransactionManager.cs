using System.Data;
using TalentHub.Integration.Sql.Abstractions;

namespace TalentHub.Integration.Sql.Transactions;

public sealed class TransactionManager(IDbConnectionFactory connectionFactory) : ITransactionManager
{
    public async Task ExecuteAsync(Func<IDbTransaction, CancellationToken, Task> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var transaction = await connection.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);

        try
        {
            await action(transaction, cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }

    public async Task<TResult> ExecuteAsync<TResult>(Func<IDbTransaction, CancellationToken, Task<TResult>> action, IsolationLevel isolationLevel = IsolationLevel.ReadCommitted, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        await using var connection = await connectionFactory.CreateOpenConnectionAsync(cancellationToken).ConfigureAwait(false);
        await using var transaction = await connection.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);

        try
        {
            var result = await action(transaction, cancellationToken).ConfigureAwait(false);
            await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            throw;
        }
    }
}
