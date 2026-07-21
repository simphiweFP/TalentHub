using TalentHub.Integration.Communication.Models;
using TalentHub.Integration.RemoteOK.Client;
using TalentHub.Integration.RemoteOK.Mapping;
using TalentHub.Integration.RemoteOK.Models;

namespace TalentHub.Integration.RemoteOK.Services;

public sealed class RemoteOkService(IRemoteOkClient client)
{
    public async Task<IReadOnlyList<Job>> GetJobsAsync(CancellationToken cancellationToken = default)
        => (await client.GetJobsAsync(cancellationToken).ConfigureAwait(false)).ToJobs();

    public async Task<IReadOnlyList<Job>> SearchJobsAsync(RemoteOkSearchRequest request, CancellationToken cancellationToken = default)
        => (await client.SearchJobsAsync(request, cancellationToken).ConfigureAwait(false)).ToJobs();

    public async Task<Job?> GetCompanyJobPreviewAsync(string externalCompanyId, CancellationToken cancellationToken = default)
    {
        var company = await client.GetCompanyAsync(externalCompanyId, cancellationToken).ConfigureAwait(false);
        if (company is null)
        {
            return null;
        }

        return new Job
        {
            ExternalId = company.Id,
            Title = company.Name,
            CompanyName = company.Name,
            CompanyWebsite = company.Website,
            Location = company.Location,
            Description = company.Description,
            IsRemote = true,
            PublishedAtUtc = DateTimeOffset.UtcNow
        };
    }
}
