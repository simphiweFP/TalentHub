using TalentHub.Integration.Communication.Abstractions;

namespace TalentHub.Integration.Communication.Services;

public sealed class CacheKeyBuilder : ICacheKeyBuilder
{
    public string Build(string namespaceName, string key)
        => $"{BuildNamespace(namespaceName)}:{key}";

    public string BuildNamespace(string namespaceName)
        => namespaceName.Trim().ToUpperInvariant();
}
