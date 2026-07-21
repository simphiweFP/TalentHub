namespace TalentHub.Integration.Communication.Abstractions;

public interface IProviderFactory
{
    IJobProvider Create(string providerName);
}
