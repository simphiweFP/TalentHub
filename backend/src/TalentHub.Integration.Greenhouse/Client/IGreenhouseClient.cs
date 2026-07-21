using TalentHub.Integration.Greenhouse.Models;

namespace TalentHub.Integration.Greenhouse.Client;

public interface IGreenhouseClient
{
    Task<IReadOnlyList<GreenhouseJobResponse>> GetJobsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GreenhouseJobResponse>> SearchJobsAsync(GreenhouseSearchRequest request, CancellationToken cancellationToken = default);

    Task<GreenhouseCompanyResponse?> GetCompanyAsync(string externalCompanyId, CancellationToken cancellationToken = default);
}
