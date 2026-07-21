using Microsoft.Extensions.DependencyInjection;

namespace TalentHub.Integration.Lever;

public static class DependencyInjection
{
    public static IServiceCollection AddLeverIntegration(this IServiceCollection services)
    {
        return services;
    }
}