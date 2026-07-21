using Microsoft.Extensions.DependencyInjection;

namespace TalentHub.Integration.Communication;

public static class DependencyInjection
{
    public static IServiceCollection AddCommunicationIntegration(this IServiceCollection services)
    {
        return services;
    }
}