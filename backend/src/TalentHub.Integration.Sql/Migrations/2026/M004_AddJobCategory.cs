using FluentMigrator;
using TalentHub.Integration.Sql.Constants;

namespace TalentHub.Integration.Sql.Migrations._2026;

/// <summary>
/// Creates the JobCategories lookup table used by jobs and search filters.
/// Dependencies: none.
/// Rollback: safe after jobs are reassigned or removed.
/// </summary>
[Migration(202607221245, "Create JobCategories table")]
public sealed class M004_AddJobCategory : Migration
{
    public override void Up()
    {
        Create.Table(TableNames.JobCategories)
            .WithColumn(ColumnNames.Id).AsInt32().Identity().NotNullable().PrimaryKey(ConstraintNames.PK_JobCategories)
            .WithColumn(ColumnNames.Name).AsString(150).NotNullable()
            .WithColumn(ColumnNames.Slug).AsString(150).NotNullable()
            .WithColumn("Description").AsString(500).Nullable();

        Create.Index("IX_JobCategories_Name")
            .OnTable(TableNames.JobCategories)
            .OnColumn(ColumnNames.Name).Ascending()
            .WithOptions().Unique();

        Create.Index("IX_JobCategories_Slug")
            .OnTable(TableNames.JobCategories)
            .OnColumn(ColumnNames.Slug).Ascending()
            .WithOptions().Unique();
    }

    public override void Down()
    {
        Delete.Table(TableNames.JobCategories);
    }
}
