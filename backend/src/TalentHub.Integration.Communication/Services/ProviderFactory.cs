using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Registry;
using TalentHub.Integration.Communication.Options;

namespace TalentHub.Integration.Communication.Services;

public sealed class ProviderFactory(IServiceProvider serviceProvider, ProviderRegistry registry, ICacheStore cacheStore, IOptions<CacheOptions> cacheOptions) : IProviderFactory
{
    public IJobProvider Create(string providerName)
    {
        if (!registry.TryGetProviderType(providerName, out var providerType))
        {
            throw new KeyNotFoundException($"Provider '{providerName}' is not registered.");
        }

        var provider = (IJobProvider)serviceProvider.GetRequiredService(providerType);
        return cacheOptions.Value.Enabled ? new CachedJobProvider(provider, cacheStore, cacheOptions) : provider;
    }
}
