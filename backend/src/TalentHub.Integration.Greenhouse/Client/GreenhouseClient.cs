using Microsoft.Extensions.Options;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Greenhouse.Exceptions;
using TalentHub.Integration.Greenhouse.Models;
using TalentHub.Integration.Greenhouse.Options;

namespace TalentHub.Integration.Greenhouse.Client;

public sealed class GreenhouseClient(ICommunicationHttpClientFactory clientFactory, IOptions<GreenhouseOptions> options) : IGreenhouseClient
{
    private readonly GreenhouseOptions _options = options.Value;
    private readonly HttpClient _client = clientFactory.CreateClient("communication");

    public Task<IReadOnlyList<GreenhouseJobResponse>> GetJobsAsync(CancellationToken cancellationToken = default)
        => Task.FromResult<IReadOnlyList<GreenhouseJobResponse>>(CreatePlaceholderJobs());

    public Task<IReadOnlyList<GreenhouseJobResponse>> SearchJobsAsync(GreenhouseSearchRequest request, CancellationToken cancellationToken = default)
    {
        if (request.PageNumber < 1 || request.PageSize < 1)
        {
            throw new GreenhouseException("invalid_pagination", "Page number and page size must be greater than zero.");
        }

        var jobs = CreatePlaceholderJobs();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            jobs = jobs.Where(job => job.Title.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase) || job.CompanyName.Contains(request.SearchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.Location))
        {
            jobs = jobs.Where(job => string.Equals(job.Location, request.Location, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        return Task.FromResult<IReadOnlyList<GreenhouseJobResponse>>(jobs.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList());
    }

    public Task<GreenhouseCompanyResponse?> GetCompanyAsync(string externalCompanyId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(externalCompanyId))
        {
            throw new GreenhouseException("invalid_company_id", "A company identifier is required.");
        }

        return Task.FromResult<GreenhouseCompanyResponse?>(new GreenhouseCompanyResponse
        {
            Id = externalCompanyId,
            Name = "Greenhouse",
            Website = _options.BaseUrl,
            Description = "Placeholder Greenhouse company",
            Location = "New York"
        });
    }

    private static List<GreenhouseJobResponse> CreatePlaceholderJobs()
        =>
        [
            new GreenhouseJobResponse
            {
                Id = "greenhouse-1",
                Title = "Full Stack Engineer",
                CompanyName = "Greenhouse",
                Location = "New York",
                Url = "https://boards.greenhouse.io/greenhouse-1",
                Content = "Placeholder job from Greenhouse.",
                IsRemote = false,
                CreatedAt = DateTimeOffset.UtcNow.AddHours(-1)
            },
            new GreenhouseJobResponse
            {
                Id = "greenhouse-2",
                Title = "Product Manager",
                CompanyName = "Greenhouse",
                Location = "New York",
                Url = "https://boards.greenhouse.io/greenhouse-2",
                Content = "Placeholder job from Greenhouse.",
                IsRemote = false,
                CreatedAt = DateTimeOffset.UtcNow.AddHours(-4)
            }
        ];
}
