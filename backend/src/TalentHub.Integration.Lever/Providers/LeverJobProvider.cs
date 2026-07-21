using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Models;

namespace TalentHub.Integration.Lever.Providers;

public sealed class LeverJobProvider : IJobProvider
{
    public string Name => "lever";

    public Task<IReadOnlyList<ProviderJob>> GetJobsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<ProviderJob>>([
            new ProviderJob("lever-1", "Backend Engineer", "Lever", "Remote", "https://example.com/lever/jobs/1", DateTimeOffset.UtcNow.AddDays(-1), true),
            new ProviderJob("lever-2", "Platform Engineer", "Lever", "Remote", "https://example.com/lever/jobs/2", DateTimeOffset.UtcNow.AddDays(-2), true)
        ]);

    public Task<IReadOnlyList<ProviderJob>> SearchJobsAsync(ProviderJobQuery query, CancellationToken cancellationToken = default)
        => GetJobsAsync(cancellationToken);

    public Task<ProviderCompany?> GetCompanyAsync(string externalCompanyId, CancellationToken cancellationToken = default)
        => Task.FromResult<ProviderCompany?>(new ProviderCompany("lever", "Lever", "https://lever.co", "Placeholder Lever company", "Remote"));
}
