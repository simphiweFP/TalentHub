using TalentHub.Integration.Lever.Models;

namespace TalentHub.Integration.Lever.Client;

public interface ILeverClient
{
    Task<IReadOnlyList<LeverJobResponse>> GetJobsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<LeverJobResponse>> SearchJobsAsync(LeverSearchRequest request, CancellationToken cancellationToken = default);

    Task<LeverCompanyResponse?> GetCompanyAsync(string externalCompanyId, CancellationToken cancellationToken = default);
}
