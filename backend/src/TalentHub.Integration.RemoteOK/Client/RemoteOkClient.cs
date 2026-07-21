using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.RemoteOK.Exceptions;
using TalentHub.Integration.RemoteOK.Models;
using TalentHub.Integration.RemoteOK.Options;
using Microsoft.Extensions.Options;

namespace TalentHub.Integration.RemoteOK.Client;

public sealed class RemoteOkClient(ICommunicationHttpClientFactory clientFactory, IOptions<RemoteOkOptions> options) : IRemoteOkClient
{
    private readonly RemoteOkOptions _options = options.Value;
    private readonly HttpClient _client = clientFactory.CreateClient("communication");

    public Task<IReadOnlyList<RemoteOkJobResponse>> GetJobsAsync(CancellationToken cancellationToken = default)
    {
        var jobs = CreatePlaceholderJobs();
        return Task.FromResult<IReadOnlyList<RemoteOkJobResponse>>(jobs);
    }

    public Task<IReadOnlyList<RemoteOkJobResponse>> SearchJobsAsync(RemoteOkSearchRequest request, CancellationToken cancellationToken = default)
    {
        if (request.PageNumber < 1 || request.PageSize < 1)
        {
            throw new RemoteOkException("invalid_pagination", "Page number and page size must be greater than zero.");
        }

        var jobs = CreatePlaceholderJobs();
        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            jobs = jobs.Where(job => job.Position.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) || job.Company.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            jobs = jobs.Where(job => string.Equals(job.Location, request.Location, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return Task.FromResult<IReadOnlyList<RemoteOkJobResponse>>(jobs.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList());
    }

    public Task<RemoteOkCompanyResponse?> GetCompanyAsync(string externalCompanyId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(externalCompanyId))
        {
            throw new RemoteOkException("invalid_company_id", "A company identifier is required.");
        }

        return Task.FromResult<RemoteOkCompanyResponse?>(new RemoteOkCompanyResponse
        {
            Id = externalCompanyId,
            Name = "RemoteOK",
            Website = _options.BaseUrl,
            Description = "Placeholder RemoteOK company",
            Location = "Remote"
        });
    }

    private static List<RemoteOkJobResponse> CreatePlaceholderJobs()
        =>
        [
            new RemoteOkJobResponse
            {
                Id = "remoteok-1",
                Position = "Senior .NET Engineer",
                Company = "RemoteOK",
                Location = "Remote",
                Url = "https://remoteok.com/remoteok-1",
                Description = "Placeholder job from RemoteOK.",
                Remote = true,
                Date = DateTimeOffset.UtcNow.AddHours(-1)
            },
            new RemoteOkJobResponse
            {
                Id = "remoteok-2",
                Position = "Staff Platform Engineer",
                Company = "RemoteOK",
                Location = "Remote",
                Url = "https://remoteok.com/remoteok-2",
                Description = "Placeholder job from RemoteOK.",
                Remote = true,
                Date = DateTimeOffset.UtcNow.AddHours(-6)
            }
        ];
}
