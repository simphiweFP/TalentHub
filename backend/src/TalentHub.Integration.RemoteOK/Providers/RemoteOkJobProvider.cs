using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Models;

namespace TalentHub.Integration.RemoteOK.Providers;

public sealed class RemoteOkJobProvider : IJobProvider
{
    public string Name => "remoteok";

    public Task<IReadOnlyList<ProviderJob>> GetJobsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<ProviderJob>>([
            new ProviderJob("remoteok-1", "Senior .NET Engineer", "RemoteOK", "Remote", "https://example.com/remoteok/jobs/1", DateTimeOffset.UtcNow.AddHours(-2), true),
            new ProviderJob("remoteok-2", "Staff Engineer", "RemoteOK", "Remote", "https://example.com/remoteok/jobs/2", DateTimeOffset.UtcNow.AddHours(-6), true)
        ]);

    public Task<IReadOnlyList<ProviderJob>> SearchJobsAsync(ProviderJobQuery query, CancellationToken cancellationToken = default)
        => GetJobsAsync(cancellationToken);

    public Task<ProviderCompany?> GetCompanyAsync(string externalCompanyId, CancellationToken cancellationToken = default)
        => Task.FromResult<ProviderCompany?>(new ProviderCompany("remoteok", "RemoteOK", "https://remoteok.com", "Placeholder RemoteOK company", "Remote"));
}
