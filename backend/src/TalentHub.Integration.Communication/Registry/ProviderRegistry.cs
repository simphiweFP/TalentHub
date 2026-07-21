using Microsoft.Extensions.Options;

namespace TalentHub.Integration.Communication.Registry;

public sealed class ProviderRegistry(IOptions<ProviderRegistryOptions> options)
{
    private readonly IReadOnlyDictionary<string, Type> _providers = options.Value.Providers
        .ToDictionary(provider => provider.Name, provider => provider.ProviderType, StringComparer.OrdinalIgnoreCase);

    public bool TryGetProviderType(string providerName, out Type providerType)
        => _providers.TryGetValue(providerName, out providerType!);

    public IReadOnlyCollection<string> GetProviderNames()
        => _providers.Keys.ToArray();
}
