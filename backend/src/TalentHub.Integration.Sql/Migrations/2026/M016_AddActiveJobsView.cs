using FluentMigrator;
using TalentHub.Integration.Sql.Views;

namespace TalentHub.Integration.Sql.Migrations._2026;

/// <summary>
/// Creates the active jobs view used for read-optimized job queries.
/// Dependencies: Companies, JobCategories, Locations, and Jobs must already exist.
/// Rollback: the view can be dropped without affecting source tables.
/// </summary>
[Migration(202607221545, "Create ActiveJobs view")]
public sealed class M016_AddActiveJobsView : Migration
{
    public override void Up() => Execute.Sql(ActiveJobsViewSql.Create);

    public override void Down() => Execute.Sql("DROP VIEW IF EXISTS [dbo].[vw_ActiveJobs];");
}
