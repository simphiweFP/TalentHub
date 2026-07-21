using Microsoft.Extensions.DependencyInjection;
using TalentHub.Integration.Communication;
using TalentHub.Integration.Lever.Providers;

namespace TalentHub.Integration.Lever;

public static class DependencyInjection
{
    public static IServiceCollection AddLeverIntegration(this IServiceCollection services)
    {
        services.AddJobProvider<LeverJobProvider>("lever");
        return services;
    }
}