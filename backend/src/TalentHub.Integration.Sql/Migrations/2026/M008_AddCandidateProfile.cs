using System.Data;
using FluentMigrator;
using TalentHub.Integration.Sql.Constants;

namespace TalentHub.Integration.Sql.Migrations._2026;

/// <summary>
/// Creates the CandidateProfiles table for candidate-specific profile data.
/// Dependencies: Users and Locations should exist before the table is used.
/// Rollback: safe after related application and saved job records are removed.
/// </summary>
[Migration(202607221345, "Create CandidateProfiles table")]
public sealed class M008_AddCandidateProfile : Migration
{
    public override void Up()
    {
        Create.Table(TableNames.CandidateProfiles)
            .WithColumn(ColumnNames.Id).AsGuid().NotNullable().PrimaryKey(ConstraintNames.PK_CandidateProfiles)
            .WithColumn(ColumnNames.UserId).AsGuid().NotNullable()
            .WithColumn("Headline").AsString(250).Nullable()
            .WithColumn("Summary").AsString(int.MaxValue).Nullable()
            .WithColumn("ResumeUrl").AsString(500).Nullable()
            .WithColumn("YearsOfExperience").AsDecimal(5, 2).Nullable()
            .WithColumn(ColumnNames.LocationId).AsGuid().Nullable()
            .WithColumn("IsOpenToWork").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn(ColumnNames.CreatedAtUtc).AsDateTime2().NotNullable()
            .WithColumn(ColumnNames.UpdatedAtUtc).AsDateTime2().Nullable();

        Create.ForeignKey("FK_CandidateProfiles_Users")
            .FromTable(TableNames.CandidateProfiles).ForeignColumn(ColumnNames.UserId)
            .ToTable(TableNames.Users).PrimaryColumn(ColumnNames.Id)
            .OnDelete(Rule.Cascade);

        Create.ForeignKey("FK_CandidateProfiles_Locations")
            .FromTable(TableNames.CandidateProfiles).ForeignColumn(ColumnNames.LocationId)
            .ToTable(TableNames.Locations).PrimaryColumn(ColumnNames.Id)
            .OnDelete(Rule.SetNull);

        Create.Index("IX_CandidateProfiles_UserId")
            .OnTable(TableNames.CandidateProfiles)
            .OnColumn(ColumnNames.UserId).Ascending()
            .WithOptions().Unique();
    }

    public override void Down()
    {
        Delete.Table(TableNames.CandidateProfiles);
    }
}
