using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TalentHub.Integration.Communication;
using TalentHub.Integration.RemoteOK.Client;
using TalentHub.Integration.RemoteOK.Providers;
using TalentHub.Integration.RemoteOK.Services;
using TalentHub.Integration.RemoteOK.Options;

namespace TalentHub.Integration.RemoteOK;

public static class DependencyInjection
{
    public static IServiceCollection AddRemoteOkIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCommunicationIntegration();
        services.AddOptions<RemoteOkOptions>().Bind(configuration.GetSection(RemoteOkOptions.SectionName));
        services.AddSingleton<IRemoteOkClient, RemoteOkClient>();
        services.AddScoped<RemoteOkService>();
        services.AddJobProvider<RemoteOkJobProvider>("remoteok");
        return services;
    }
}