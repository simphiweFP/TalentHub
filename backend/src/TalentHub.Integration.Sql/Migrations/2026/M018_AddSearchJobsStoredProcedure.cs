using FluentMigrator;
using TalentHub.Integration.Sql.StoredProcedures;

namespace TalentHub.Integration.Sql.Migrations._2026;

/// <summary>
/// Creates the search jobs stored procedure for paged read scenarios.
/// Dependencies: Jobs, Companies, JobCategories, and the normalize function must already exist.
/// Rollback: safe because callers can fall back to view-based queries afterward.
/// </summary>
[Migration(202607221615, "Create search jobs stored procedure")]
public sealed class M018_AddSearchJobsStoredProcedure : Migration
{
    public override void Up() => Execute.Sql(SearchJobsSql.Create);

    public override void Down() => Execute.Sql("DROP PROCEDURE IF EXISTS [dbo].[usp_SearchJobs];");
}
