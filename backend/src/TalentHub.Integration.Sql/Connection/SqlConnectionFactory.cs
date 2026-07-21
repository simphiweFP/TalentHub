using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using TalentHub.Integration.Sql.Abstractions;
using TalentHub.Integration.Sql.Options;

namespace TalentHub.Integration.Sql.Connection;

public sealed class SqlConnectionFactory(IOptions<SqlConnectionOptions> options) : IDbConnectionFactory
{
    private readonly SqlConnectionOptions _options = options.Value;

    public SqlConnection CreateConnection()
        => new(GetConnectionString());

    public async Task<SqlConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = CreateConnection();
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        return connection;
    }

    private string GetConnectionString()
    {
        if (!string.IsNullOrWhiteSpace(_options.ConnectionString))
        {
            return _options.ConnectionString;
        }

        throw new InvalidOperationException("A SQL Server connection string was not configured.");
    }
}
