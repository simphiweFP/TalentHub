namespace TalentHub.Web.API.Options;

public sealed class BackgroundWorkerOptions
{
    public const string SectionName = "BackgroundWorkers";

    public TimeSpan JobSyncInterval { get; set; } = TimeSpan.FromMinutes(15);

    public TimeSpan CompanyRefreshInterval { get; set; } = TimeSpan.FromHours(1);

    public TimeSpan ExpiredDataCleanupInterval { get; set; } = TimeSpan.FromHours(6);

    public int RetryCount { get; set; } = 3;

    public int RetryDelayMilliseconds { get; set; } = 1000;

    public int SearchHistoryRetentionDays { get; set; } = 30;

    public int AuditLogRetentionDays { get; set; } = 90;

    public int ClosedJobRetentionDays { get; set; } = 30;
}
