namespace TalentHub.Integration.Communication.Services;

public static class CacheNamespaces
{
    public const string AggregationJobs = "aggregation:jobs";
    public const string ProviderJobs = "provider:jobs";
    public const string ProviderSearch = "provider:search";
    public const string ProviderCompany = "provider:company";

    public static string ForProviderJobs(string providerName)
        => $"{ProviderJobs}:{Normalize(providerName)}";

    public static string ForProviderSearch(string providerName)
        => $"{ProviderSearch}:{Normalize(providerName)}";

    public static string ForProviderCompany(string providerName)
        => $"{ProviderCompany}:{Normalize(providerName)}";

    private static string Normalize(string value)
        => string.IsNullOrWhiteSpace(value)
            ? string.Empty
            : value.Trim().ToUpperInvariant();
}
