namespace TalentHub.Integration.Communication.Options;

public sealed class CacheOptions
{
    public const string SectionName = "Caching";

    public bool Enabled { get; set; } = true;

    public bool UseDistributedCache { get; set; }

    public TimeSpan ProviderJobsExpiration { get; set; } = TimeSpan.FromMinutes(15);

    public TimeSpan ProviderSearchExpiration { get; set; } = TimeSpan.FromMinutes(5);

    public TimeSpan ProviderCompanyExpiration { get; set; } = TimeSpan.FromHours(1);

    public TimeSpan AggregationExpiration { get; set; } = TimeSpan.FromMinutes(10);
}
