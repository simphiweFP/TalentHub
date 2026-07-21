using Microsoft.Data.SqlClient;

namespace TalentHub.Integration.Sql.Abstractions;

public interface IDbConnectionFactory
{
    SqlConnection CreateConnection();

    Task<SqlConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}
