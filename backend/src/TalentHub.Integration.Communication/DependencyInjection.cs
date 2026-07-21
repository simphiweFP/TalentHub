using Microsoft.Extensions.DependencyInjection;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Clients;
using TalentHub.Integration.Communication.Services;

namespace TalentHub.Integration.Communication;

public static class DependencyInjection
{
    public static IServiceCollection AddCommunicationIntegration(this IServiceCollection services)
    {
        services.AddTransient<IBaseApiClient, BaseApiClientProxy>();

        return services;
    }

    private sealed class BaseApiClientProxy(
        ICommunicationHttpClientFactory clientFactory,
        IJsonSerializer serializer) : BaseApiClient(clientFactory, serializer)
    {
    }
}