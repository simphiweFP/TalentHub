using TalentHub.Integration.Communication.Models;
using TalentHub.Integration.Greenhouse.Client;
using TalentHub.Integration.Greenhouse.Mapping;
using TalentHub.Integration.Greenhouse.Models;

namespace TalentHub.Integration.Greenhouse.Services;

public sealed class GreenhouseService(IGreenhouseClient client)
{
    public async Task<IReadOnlyList<Job>> GetJobsAsync(CancellationToken cancellationToken = default)
        => (await client.GetJobsAsync(cancellationToken).ConfigureAwait(false)).ToJobs();

    public async Task<IReadOnlyList<Job>> SearchJobsAsync(GreenhouseSearchRequest request, CancellationToken cancellationToken = default)
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
            Source = "Greenhouse",
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
