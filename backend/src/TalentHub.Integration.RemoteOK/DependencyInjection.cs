using Microsoft.Extensions.DependencyInjection;
using TalentHub.Integration.Communication;
using TalentHub.Integration.RemoteOK.Providers;

namespace TalentHub.Integration.RemoteOK;

public static class DependencyInjection
{
    public static IServiceCollection AddRemoteOkIntegration(this IServiceCollection services)
    {
        services.AddJobProvider<RemoteOkJobProvider>("remoteok");
        return services;
    }
}