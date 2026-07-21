using Microsoft.Extensions.DependencyInjection;

namespace TalentHub.Integration.Sql;

public static class DependencyInjection
{
    public static IServiceCollection AddSqlIntegration(this IServiceCollection services)
    {
        return services;
    }
}