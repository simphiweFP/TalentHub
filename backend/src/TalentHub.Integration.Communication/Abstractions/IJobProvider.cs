using TalentHub.Integration.Communication.Models;

namespace TalentHub.Integration.Communication.Abstractions;

public interface IJobProvider
{
    string Name { get; }

    Task<IReadOnlyList<ProviderJob>> GetJobsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ProviderJob>> SearchJobsAsync(ProviderJobQuery query, CancellationToken cancellationToken = default);

    Task<ProviderCompany?> GetCompanyAsync(string externalCompanyId, CancellationToken cancellationToken = default);
}
