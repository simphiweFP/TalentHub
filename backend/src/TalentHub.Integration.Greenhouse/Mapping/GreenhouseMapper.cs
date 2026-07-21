using TalentHub.Integration.Communication.Models;
using TalentHub.Integration.Greenhouse.Models;

namespace TalentHub.Integration.Greenhouse.Mapping;

public static class GreenhouseMapper
{
    public static Job ToJob(this GreenhouseJobResponse response)
        => new()
        {
            Source = "Greenhouse",
            ExternalId = response.Id,
            Title = response.Title,
            CompanyName = response.CompanyName,
            Location = response.Location,
            Url = response.Url,
            Description = response.Content,
            IsRemote = response.IsRemote,
            PublishedAtUtc = response.CreatedAt ?? DateTimeOffset.UtcNow
        };

    public static IReadOnlyList<Job> ToJobs(this IEnumerable<GreenhouseJobResponse> responses)
        => responses.Select(response => response.ToJob()).ToList();
}
