using TalentHub.Integration.RemoteOK.Models;

namespace TalentHub.Integration.RemoteOK.Client;

public interface IRemoteOkClient
{
    Task<IReadOnlyList<RemoteOkJobResponse>> GetJobsAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RemoteOkJobResponse>> SearchJobsAsync(RemoteOkSearchRequest request, CancellationToken cancellationToken = default);

    Task<RemoteOkCompanyResponse?> GetCompanyAsync(string externalCompanyId, CancellationToken cancellationToken = default);
}
