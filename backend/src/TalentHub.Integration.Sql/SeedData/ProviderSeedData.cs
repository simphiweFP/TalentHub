namespace TalentHub.Integration.Sql.SeedData;

public static class ProviderSeedData
{
    private static readonly DateTime SeededAtUtc = new(2026, 7, 22, 15, 15, 0, DateTimeKind.Utc);

    public static readonly object[] Rows =
    [
        new { Id = 1, Name = "RemoteOK", Slug = "remoteok", IsActive = true, CreatedAtUtc = SeededAtUtc },
        new { Id = 2, Name = "Greenhouse", Slug = "greenhouse", IsActive = true, CreatedAtUtc = SeededAtUtc },
        new { Id = 3, Name = "Lever", Slug = "lever", IsActive = true, CreatedAtUtc = SeededAtUtc }
    ];
}
