using System.Data;
using FluentMigrator;
using TalentHub.Integration.Sql.Constants;

namespace TalentHub.Integration.Sql.Migrations._2026;

/// <summary>
/// Creates the Jobs table as the primary job posting aggregate.
/// Dependencies: Companies, JobCategories, and Locations should exist before use.
/// Rollback: safe after dependent records are removed.
/// </summary>
[Migration(202607221330, "Create Jobs table")]
public sealed class M007_AddJob : Migration
{
    public override void Up()
    {
        Create.Table(TableNames.Jobs)
            .WithColumn(ColumnNames.Id).AsGuid().NotNullable().PrimaryKey(ConstraintNames.PK_Jobs)
            .WithColumn(ColumnNames.CompanyId).AsGuid().NotNullable()
            .WithColumn(ColumnNames.JobCategoryId).AsInt32().NotNullable()
            .WithColumn(ColumnNames.LocationId).AsGuid().Nullable()
            .WithColumn("Title").AsString(200).NotNullable()
            .WithColumn(ColumnNames.Slug).AsString(250).NotNullable()
            .WithColumn("Description").AsString(int.MaxValue).Nullable()
            .WithColumn("EmploymentType").AsInt32().NotNullable()
            .WithColumn("WorkMode").AsInt32().NotNullable()
            .WithColumn("SeniorityLevel").AsInt32().NotNullable()
            .WithColumn("SalaryMin").AsDecimal(18, 2).Nullable()
            .WithColumn("SalaryMax").AsDecimal(18, 2).Nullable()
            .WithColumn("CurrencyCode").AsString(3).Nullable()
            .WithColumn("IsRemote").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn(ColumnNames.IsActive).AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("PublishedAtUtc").AsDateTime2().Nullable()
            .WithColumn("ClosedAtUtc").AsDateTime2().Nullable()
            .WithColumn(ColumnNames.CreatedAtUtc).AsDateTime2().NotNullable()
            .WithColumn(ColumnNames.UpdatedAtUtc).AsDateTime2().Nullable();

        Create.ForeignKey("FK_Jobs_Companies")
            .FromTable(TableNames.Jobs).ForeignColumn(ColumnNames.CompanyId)
            .ToTable(TableNames.Companies).PrimaryColumn(ColumnNames.Id)
            .OnDelete(Rule.Cascade);

        Create.ForeignKey("FK_Jobs_JobCategories")
            .FromTable(TableNames.Jobs).ForeignColumn(ColumnNames.JobCategoryId)
            .ToTable(TableNames.JobCategories).PrimaryColumn(ColumnNames.Id)
            .OnDelete(Rule.None);

        Create.ForeignKey("FK_Jobs_Locations")
            .FromTable(TableNames.Jobs).ForeignColumn(ColumnNames.LocationId)
            .ToTable(TableNames.Locations).PrimaryColumn(ColumnNames.Id)
            .OnDelete(Rule.SetNull);

        Create.Index(IndexNames.IX_Jobs_CompanyId)
            .OnTable(TableNames.Jobs)
            .OnColumn(ColumnNames.CompanyId).Ascending()
            .WithOptions().NonClustered();

        Create.Index(IndexNames.IX_Jobs_CategoryId)
            .OnTable(TableNames.Jobs)
            .OnColumn(ColumnNames.JobCategoryId).Ascending()
            .WithOptions().NonClustered();

        Create.Index(IndexNames.IX_Jobs_IsActive)
            .OnTable(TableNames.Jobs)
            .OnColumn(ColumnNames.IsActive).Ascending()
            .WithOptions().NonClustered();

        Create.Index("IX_Jobs_CompanyId_Slug")
            .OnTable(TableNames.Jobs)
            .OnColumn(ColumnNames.CompanyId).Ascending()
            .OnColumn(ColumnNames.Slug).Ascending()
            .WithOptions().Unique();
    }

    public override void Down()
    {
        Delete.Table(TableNames.Jobs);
    }
}
