using Microsoft.Extensions.DependencyInjection;

namespace TalentHub.Integration.Greenhouse;

public static class DependencyInjection
{
    public static IServiceCollection AddGreenhouseIntegration(this IServiceCollection services)
    {
        return services;
    }
}