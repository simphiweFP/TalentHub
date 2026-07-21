using FluentMigrator;
using TalentHub.Integration.Sql.Constants;

namespace TalentHub.Integration.Sql.Migrations._2026;

/// <summary>
/// Creates the Companies table used to store employer organizations.
/// Dependencies: none.
/// Rollback: the table can be safely removed when downstream tables no longer depend on it.
/// </summary>
[Migration(202607221200, "Create Companies table")]
public sealed class M001_AddCompany : Migration
{
    public override void Up()
    {
        Create.Table(TableNames.Companies)
            .WithColumn(ColumnNames.Id).AsGuid().NotNullable().PrimaryKey(ConstraintNames.PK_Companies)
            .WithColumn(ColumnNames.Name).AsString(200).NotNullable()
            .WithColumn(ColumnNames.Slug).AsString(200).NotNullable()
            .WithColumn("WebsiteUrl").AsString(500).Nullable()
            .WithColumn("Description").AsString(int.MaxValue).Nullable()
            .WithColumn("Industry").AsString(150).Nullable()
            .WithColumn(ColumnNames.IsActive).AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn(ColumnNames.CreatedAtUtc).AsDateTime2().NotNullable()
            .WithColumn(ColumnNames.UpdatedAtUtc).AsDateTime2().Nullable();

        Create.Index("IX_Companies_Slug")
            .OnTable(TableNames.Companies)
            .OnColumn(ColumnNames.Slug).Ascending()
            .WithOptions().Unique();

        Create.Index(IndexNames.IX_Companies_Name)
            .OnTable(TableNames.Companies)
            .OnColumn(ColumnNames.Name).Ascending()
            .WithOptions().NonClustered();
    }

    public override void Down()
    {
        Delete.Table(TableNames.Companies);
    }
}
