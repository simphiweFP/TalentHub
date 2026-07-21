using Microsoft.Extensions.DependencyInjection;
using TalentHub.Integration.Communication;
using TalentHub.Integration.Greenhouse.Providers;

namespace TalentHub.Integration.Greenhouse;

public static class DependencyInjection
{
    public static IServiceCollection AddGreenhouseIntegration(this IServiceCollection services)
    {
        services.AddJobProvider<GreenhouseJobProvider>("greenhouse");
        return services;
    }
}