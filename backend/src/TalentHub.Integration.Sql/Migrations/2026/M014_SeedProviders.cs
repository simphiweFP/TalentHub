using FluentMigrator;
using TalentHub.Integration.Sql.Constants;
using TalentHub.Integration.Sql.SeedData;

namespace TalentHub.Integration.Sql.Migrations._2026;

/// <summary>
/// Seeds the Providers lookup data required for external integrations.
/// Dependencies: Providers table must exist.
/// Rollback: rows can be safely deleted by identifier.
/// </summary>
[Migration(202607221515, "Seed Providers")]
public sealed class M014_SeedProviders : Migration
{
    public override void Up()
    {
        Insert.IntoTable(TableNames.Providers)
            .Row(ProviderSeedData.Rows[0])
            .Row(ProviderSeedData.Rows[1])
            .Row(ProviderSeedData.Rows[2]);
    }

    public override void Down()
    {
        Delete.FromTable(TableNames.Providers).Row(new { Id = 1 });
        Delete.FromTable(TableNames.Providers).Row(new { Id = 2 });
        Delete.FromTable(TableNames.Providers).Row(new { Id = 3 });
    }
}
