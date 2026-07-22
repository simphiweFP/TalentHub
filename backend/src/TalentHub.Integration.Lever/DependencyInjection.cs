using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TalentHub.Integration.Communication;
using TalentHub.Integration.Lever.Client;
using TalentHub.Integration.Lever.Options;
using TalentHub.Integration.Lever.Providers;
using TalentHub.Integration.Lever.Services;

namespace TalentHub.Integration.Lever;

public static class DependencyInjection
{
    public static IServiceCollection AddLeverIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddCommunicationIntegration();
        services.AddOptions<LeverOptions>().Bind(configuration.GetSection(LeverOptions.SectionName));
        services.AddSingleton<ILeverClient, LeverClient>();
        services.AddScoped<LeverService>();
        services.AddJobProvider<LeverJobProvider>("lever");
        return services;
    }
}