namespace TalentHub.Integration.Communication.Abstractions;

public interface IProviderResolver
{
    IJobProvider Resolve(string providerName);

    IReadOnlyCollection<string> GetProviderNames();
}
