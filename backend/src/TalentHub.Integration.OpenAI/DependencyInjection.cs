using Microsoft.Extensions.DependencyInjection;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.OpenAI.Providers;

namespace TalentHub.Integration.OpenAI;

public static class DependencyInjection
{
    public static IServiceCollection AddOpenAiIntegration(this IServiceCollection services)
    {
        services.AddSingleton<IAIProvider, OpenAiProvider>();
        return services;
    }
}