using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Models;

namespace TalentHub.Integration.Greenhouse.Providers;

public sealed class GreenhouseJobProvider : IJobProvider
{
    public string Name => "greenhouse";

    public Task<IReadOnlyList<ProviderJob>> GetJobsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<ProviderJob>>([
            new ProviderJob("greenhouse-1", "Full Stack Engineer", "Greenhouse", "New York", "https://example.com/greenhouse/jobs/1", DateTimeOffset.UtcNow.AddDays(-1), false),
            new ProviderJob("greenhouse-2", "Product Manager", "Greenhouse", "New York", "https://example.com/greenhouse/jobs/2", DateTimeOffset.UtcNow.AddDays(-3), false)
        ]);

    public Task<IReadOnlyList<ProviderJob>> SearchJobsAsync(ProviderJobQuery query, CancellationToken cancellationToken = default)
        => GetJobsAsync(cancellationToken);

    public Task<ProviderCompany?> GetCompanyAsync(string externalCompanyId, CancellationToken cancellationToken = default)
        => Task.FromResult<ProviderCompany?>(new ProviderCompany("greenhouse", "Greenhouse", "https://greenhouse.com", "Placeholder Greenhouse company", "New York"));
}
