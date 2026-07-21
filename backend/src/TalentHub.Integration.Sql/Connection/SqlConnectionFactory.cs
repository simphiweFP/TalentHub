using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using TalentHub.Integration.Sql.Abstractions;
using TalentHub.Integration.Sql.Options;

namespace TalentHub.Integration.Sql.Connection;

public sealed class SqlConnectionFactory(IOptions<SqlConnectionOptions> options, IConfiguration configuration) : IDbConnectionFactory
{
    private readonly SqlConnectionOptions _options = options.Value;
    private readonly IConfiguration _configuration = configuration;

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

        var configuredConnectionString = _configuration.GetConnectionString(_options.ConnectionStringName);
        if (!string.IsNullOrWhiteSpace(configuredConnectionString))
        {
            return configuredConnectionString;
        }

        throw new InvalidOperationException("A SQL Server connection string was not configured.");
    }
}
