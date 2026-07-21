using TalentHub.Integration.Communication.Abstractions;

namespace TalentHub.Integration.Communication.Registry;

public sealed record ProviderRegistration(string Name, Type ProviderType);
