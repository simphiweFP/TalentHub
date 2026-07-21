using Microsoft.Extensions.DependencyInjection;

namespace TalentHub.Integration.OpenAI;

public static class DependencyInjection
{
    public static IServiceCollection AddOpenAiIntegration(this IServiceCollection services)
    {
        return services;
    }
}