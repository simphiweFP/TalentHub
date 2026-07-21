using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Registry;

namespace TalentHub.Integration.Communication.Services;

public sealed class ProviderResolver(IProviderFactory factory, ProviderRegistry registry) : IProviderResolver
{
    public IJobProvider Resolve(string providerName) => factory.Create(providerName);

    public IReadOnlyCollection<string> GetProviderNames() => registry.GetProviderNames();
}
