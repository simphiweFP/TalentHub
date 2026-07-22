using TalentHub.Integration.Communication.Models;
using TalentHub.Integration.Lever.Client;
using TalentHub.Integration.Lever.Mapping;
using TalentHub.Integration.Lever.Models;

namespace TalentHub.Integration.Lever.Services;

public sealed class LeverService(ILeverClient client)
{
    public async Task<IReadOnlyList<Job>> GetJobsAsync(CancellationToken cancellationToken = default)
        => (await client.GetJobsAsync(cancellationToken).ConfigureAwait(false)).ToJobs();

    public async Task<IReadOnlyList<Job>> SearchJobsAsync(LeverSearchRequest request, CancellationToken cancellationToken = default)
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
            Source = "Lever",
            ExternalId = company.Id,
            Title = company.Name,
            CompanyName = company.Name,
            CompanyWebsite = company.Website,
            Location = company.Location,
            Description = company.Description,
            IsRemote = false,
            PublishedAtUtc = DateTimeOffset.UtcNow
        };
    }
}
