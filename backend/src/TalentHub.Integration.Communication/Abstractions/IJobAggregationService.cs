using TalentHub.Integration.Communication.Models;

namespace TalentHub.Integration.Communication.Abstractions;

public interface IJobAggregationService
{
    Task<JobAggregationResult> GetJobsAsync(CancellationToken cancellationToken = default);
}
