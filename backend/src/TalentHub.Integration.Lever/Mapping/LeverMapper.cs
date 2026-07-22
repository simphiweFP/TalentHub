using TalentHub.Integration.Communication.Models;
using TalentHub.Integration.Lever.Models;

namespace TalentHub.Integration.Lever.Mapping;

public static class LeverMapper
{
    public static Job ToJob(this LeverJobResponse response)
        => new()
        {
            Source = "Lever",
            ExternalId = response.Id,
            Title = response.Text,
            CompanyName = response.Lever,
            Location = response.Location,
            Url = response.Url,
            Description = response.Description,
            IsRemote = response.Remote,
            PublishedAtUtc = response.CreatedAt ?? DateTimeOffset.UtcNow
        };

    public static IReadOnlyList<Job> ToJobs(this IEnumerable<LeverJobResponse> responses)
        => responses.Select(response => response.ToJob()).ToList();
}
