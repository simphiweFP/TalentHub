using Microsoft.Extensions.DependencyInjection;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Registry;

namespace TalentHub.Integration.Communication.Services;

public sealed class ProviderFactory(IServiceProvider serviceProvider, ProviderRegistry registry) : IProviderFactory
{
    public IJobProvider Create(string providerName)
    {
        if (!registry.TryGetProviderType(providerName, out var providerType))
        {
            throw new KeyNotFoundException($"Provider '{providerName}' is not registered.");
        }

        return (IJobProvider)serviceProvider.GetRequiredService(providerType);
    }
}
