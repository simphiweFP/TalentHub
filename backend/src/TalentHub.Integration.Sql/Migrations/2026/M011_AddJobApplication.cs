using System.Data;
using FluentMigrator;
using TalentHub.Integration.Sql.Constants;

namespace TalentHub.Integration.Sql.Migrations._2026;

/// <summary>
/// Creates the JobApplications table to track candidate applications over time.
/// Dependencies: Jobs, CandidateProfiles, and Users must exist before application tracking is used.
/// Rollback: safe after related application history is no longer needed.
/// </summary>
[Migration(202607221430, "Create JobApplications table")]
public sealed class M011_AddJobApplication : Migration
{
    public override void Up()
    {
        Create.Table(TableNames.JobApplications)
            .WithColumn(ColumnNames.Id).AsGuid().NotNullable().PrimaryKey(ConstraintNames.PK_JobApplications)
            .WithColumn(ColumnNames.JobId).AsGuid().NotNullable()
            .WithColumn(ColumnNames.UserId).AsGuid().NotNullable()
            .WithColumn(ColumnNames.ProviderId).AsInt32().Nullable()
            .WithColumn("ApplicationStatus").AsInt32().NotNullable()
            .WithColumn("CoverLetter").AsString(int.MaxValue).Nullable()
            .WithColumn("ResumeUrl").AsString(500).Nullable()
            .WithColumn("Source").AsString(150).Nullable()
            .WithColumn("ReviewedAtUtc").AsDateTime2().Nullable()
            .WithColumn("AppliedAtUtc").AsDateTime2().NotNullable()
            .WithColumn(ColumnNames.CreatedAtUtc).AsDateTime2().NotNullable();

        Create.ForeignKey("FK_JobApplications_Jobs")
            .FromTable(TableNames.JobApplications).ForeignColumn(ColumnNames.JobId)
            .ToTable(TableNames.Jobs).PrimaryColumn(ColumnNames.Id)
            .OnDelete(Rule.Cascade);

        Create.ForeignKey("FK_JobApplications_Users")
            .FromTable(TableNames.JobApplications).ForeignColumn(ColumnNames.UserId)
            .ToTable(TableNames.Users).PrimaryColumn(ColumnNames.Id)
            .OnDelete(Rule.Cascade);

        Create.ForeignKey("FK_JobApplications_Providers")
            .FromTable(TableNames.JobApplications).ForeignColumn(ColumnNames.ProviderId)
            .ToTable(TableNames.Providers).PrimaryColumn(ColumnNames.Id)
            .OnDelete(Rule.SetNull);

        Create.Index("IX_JobApplications_JobId_UserId")
            .OnTable(TableNames.JobApplications)
            .OnColumn(ColumnNames.JobId).Ascending()
            .OnColumn(ColumnNames.UserId).Ascending()
            .WithOptions().Unique();
    }

    public override void Down()
    {
        Delete.Table(TableNames.JobApplications);
    }
}
