namespace TalentHub.Integration.Communication.Abstractions;

public interface ICacheKeyBuilder
{
    string Build(string namespaceName, string key);

    string BuildNamespace(string namespaceName);
}
