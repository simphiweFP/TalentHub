using Microsoft.Extensions.DependencyInjection;

namespace TalentHub.Tests;

public static class DependencyInjection
{
    public static IServiceCollection AddTestInfrastructure(this IServiceCollection services)
    {
        return services;
    }
}