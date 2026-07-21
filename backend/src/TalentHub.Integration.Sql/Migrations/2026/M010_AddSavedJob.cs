using System.Data;
using FluentMigrator;
using TalentHub.Integration.Sql.Constants;

namespace TalentHub.Integration.Sql.Migrations._2026;

/// <summary>
/// Creates the SavedJobs table so users can bookmark jobs.
/// Dependencies: Users and Jobs must exist before saved job relationships are used.
/// Rollback: safe after saved-job references are removed.
/// </summary>
[Migration(202607221415, "Create SavedJobs table")]
public sealed class M010_AddSavedJob : Migration
{
    public override void Up()
    {
        Create.Table(TableNames.SavedJobs)
            .WithColumn(ColumnNames.Id).AsGuid().NotNullable().PrimaryKey(ConstraintNames.PK_SavedJobs)
            .WithColumn(ColumnNames.UserId).AsGuid().NotNullable()
            .WithColumn(ColumnNames.JobId).AsGuid().NotNullable()
            .WithColumn("SavedAtUtc").AsDateTime2().NotNullable();

        Create.ForeignKey("FK_SavedJobs_Users")
            .FromTable(TableNames.SavedJobs).ForeignColumn(ColumnNames.UserId)
            .ToTable(TableNames.Users).PrimaryColumn(ColumnNames.Id)
            .OnDelete(Rule.Cascade);

        Create.ForeignKey("FK_SavedJobs_Jobs")
            .FromTable(TableNames.SavedJobs).ForeignColumn(ColumnNames.JobId)
            .ToTable(TableNames.Jobs).PrimaryColumn(ColumnNames.Id)
            .OnDelete(Rule.Cascade);

        Create.Index(IndexNames.IX_SavedJobs_UserId_JobId)
            .OnTable(TableNames.SavedJobs)
            .OnColumn(ColumnNames.UserId).Ascending()
            .OnColumn(ColumnNames.JobId).Ascending()
            .WithOptions().Unique();
    }

    public override void Down()
    {
        Delete.Table(TableNames.SavedJobs);
    }
}
