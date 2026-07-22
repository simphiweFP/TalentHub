using Microsoft.Extensions.Options;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Lever.Exceptions;
using TalentHub.Integration.Lever.Models;
using TalentHub.Integration.Lever.Options;

namespace TalentHub.Integration.Lever.Client;

public sealed class LeverClient(ICommunicationHttpClientFactory clientFactory, IOptions<LeverOptions> options) : ILeverClient
{
    private readonly LeverOptions _options = options.Value;
    private readonly HttpClient _client = clientFactory.CreateClient("communication");

    public Task<IReadOnlyList<LeverJobResponse>> GetJobsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<LeverJobResponse>>(CreatePlaceholderJobs());

    public Task<IReadOnlyList<LeverJobResponse>> SearchJobsAsync(LeverSearchRequest request, CancellationToken cancellationToken = default)
    {
        if (request.PageNumber < 1 || request.PageSize < 1)
        {
            throw new LeverException("invalid_pagination", "Page number and page size must be greater than zero.");
        }

        var jobs = CreatePlaceholderJobs();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            jobs = jobs.Where(job => job.Text.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) || job.Lever.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            jobs = jobs.Where(job => string.Equals(job.Location, request.Location, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return Task.FromResult<IReadOnlyList<LeverJobResponse>>(jobs.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList());
    }

    public Task<LeverCompanyResponse?> GetCompanyAsync(string externalCompanyId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(externalCompanyId))
        {
            throw new LeverException("invalid_company_id", "A company identifier is required.");
        }

        return Task.FromResult<LeverCompanyResponse?>(new LeverCompanyResponse
        {
            Id = externalCompanyId,
            Name = "Lever",
            Website = _options.BaseUrl,
            Description = "Placeholder Lever company",
            Location = "Remote"
        });
    }

    private static List<LeverJobResponse> CreatePlaceholderJobs()
        =>
        [
            new LeverJobResponse
            {
                Id = "lever-1",
                Text = "Backend Engineer",
                Lever = "Lever",
                Categories = "Engineering",
                Commitment = "Full-time",
                Location = "Remote",
                Url = "https://jobs.lever.co/lever/lever-1",
                Description = "Placeholder job from Lever.",
                Remote = true,
                CreatedAt = DateTimeOffset.UtcNow.AddHours(-1)
            },
            new LeverJobResponse
            {
                Id = "lever-2",
                Text = "Platform Engineer",
                Lever = "Lever",
                Categories = "Engineering",
                Commitment = "Full-time",
                Location = "Remote",
                Url = "https://jobs.lever.co/lever/lever-2",
                Description = "Placeholder job from Lever.",
                Remote = true,
                CreatedAt = DateTimeOffset.UtcNow.AddHours(-4)
            }
        ];
}
