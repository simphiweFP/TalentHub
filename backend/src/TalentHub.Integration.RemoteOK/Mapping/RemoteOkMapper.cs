using TalentHub.Integration.RemoteOK.Models;

namespace TalentHub.Integration.RemoteOK.Mapping;

public static class RemoteOkMapper
{
    public static Job ToJob(this RemoteOkJobResponse response)
        => new()
        {
            ExternalId = response.Id,
            Title = response.Position,
            CompanyName = response.Company,
            Location = response.Location,
            Url = response.Url,
            Description = response.Description,
            IsRemote = response.Remote,
            PublishedAtUtc = response.Date ?? DateTimeOffset.UtcNow
        };

    public static IReadOnlyList<Job> ToJobs(this IEnumerable<RemoteOkJobResponse> responses)
        => responses.Select(response => response.ToJob()).ToList();
}
