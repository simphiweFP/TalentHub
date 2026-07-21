using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TalentHub.Integration.Sql.Abstractions;
using TalentHub.Integration.Sql.Configuration;
using TalentHub.Integration.Sql.Connection;
using TalentHub.Integration.Sql.Executors;
using TalentHub.Integration.Sql.Options;
using TalentHub.Integration.Sql.Transactions;

namespace TalentHub.Integration.Sql;

public static class DependencyInjection
{
    public static IServiceCollection AddSqlIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        DapperConfiguration.Configure();

        services.AddOptions<SqlConnectionOptions>()
            .Bind(configuration.GetSection(SqlConnectionOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString), "Sql:ConnectionString must be configured.")
            .ValidateOnStart();

        services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<IQueryExecutor, QueryExecutor>();
        services.AddScoped<ICommandExecutor, CommandExecutor>();
        services.AddScoped<IStoredProcedureExecutor, StoredProcedureExecutor>();
        services.AddScoped<ITransactionManager, TransactionManager>();

        return services;
    }
}