using FluentMigrator;
using TalentHub.Integration.Sql.Constants;
using TalentHub.Integration.Sql.SeedData;

namespace TalentHub.Integration.Sql.Migrations._2026;

/// <summary>
/// Seeds the JobCategories lookup data required for initial job classification.
/// Dependencies: JobCategories table must exist.
/// Rollback: rows can be safely deleted by identifier.
/// </summary>
[Migration(202607221530, "Seed JobCategories")]
public sealed class M015_SeedJobCategories : Migration
{
    public override void Up()
    {
        Insert.IntoTable(TableNames.JobCategories)
            .Row(JobCategorySeedData.Rows[0])
            .Row(JobCategorySeedData.Rows[1])
            .Row(JobCategorySeedData.Rows[2]);
    }

    public override void Down()
    {
        Delete.FromTable(TableNames.JobCategories).Row(new { Id = 1 });
        Delete.FromTable(TableNames.JobCategories).Row(new { Id = 2 });
        Delete.FromTable(TableNames.JobCategories).Row(new { Id = 3 });
    }
}
