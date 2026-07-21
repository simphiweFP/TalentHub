namespace TalentHub.Integration.Communication.Registry;

public sealed class ProviderRegistryOptions
{
    public IList<ProviderRegistration> Providers { get; } = [];
}
