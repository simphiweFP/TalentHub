using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TalentHub.Integration.Communication;
using TalentHub.Integration.Greenhouse.Client;
using TalentHub.Integration.Greenhouse.Options;
using TalentHub.Integration.Greenhouse.Providers;
using TalentHub.Integration.Greenhouse.Services;

namespace TalentHub.Integration.Greenhouse;

public static class DependencyInjection
{
    public static IServiceCollection AddGreenhouseIntegration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<GreenhouseOptions>().Bind(configuration.GetSection(GreenhouseOptions.SectionName));
        services.AddSingleton<IGreenhouseClient, GreenhouseClient>();
        services.AddScoped<GreenhouseService>();
        services.AddJobProvider<GreenhouseJobProvider>("greenhouse");
        return services;
    }
}