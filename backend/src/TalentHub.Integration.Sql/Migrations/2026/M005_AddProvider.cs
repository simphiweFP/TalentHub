using FluentMigrator;
using TalentHub.Integration.Sql.Constants;

namespace TalentHub.Integration.Sql.Migrations._2026;

/// <summary>
/// Creates the Providers table used to track external job source integrations.
/// Dependencies: none.
/// Rollback: safe after provider-job links are removed.
/// </summary>
[Migration(202607221300, "Create Providers table")]
public sealed class M005_AddProvider : Migration
{
    public override void Up()
    {
        Create.Table(TableNames.Providers)
            .WithColumn(ColumnNames.Id).AsInt32().Identity().NotNullable().PrimaryKey(ConstraintNames.PK_Providers)
            .WithColumn(ColumnNames.Name).AsString(150).NotNullable()
            .WithColumn(ColumnNames.Slug).AsString(150).NotNullable()
            .WithColumn("WebsiteUrl").AsString(500).Nullable()
            .WithColumn("ExternalIdentifier").AsString(200).Nullable()
            .WithColumn(ColumnNames.IsActive).AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn(ColumnNames.CreatedAtUtc).AsDateTime2().NotNullable();

        Create.Index("IX_Providers_Name")
            .OnTable(TableNames.Providers)
            .OnColumn(ColumnNames.Name).Ascending()
            .WithOptions().Unique();

        Create.Index("IX_Providers_Slug")
            .OnTable(TableNames.Providers)
            .OnColumn(ColumnNames.Slug).Ascending()
            .WithOptions().Unique();
    }

    public override void Down()
    {
        Delete.Table(TableNames.Providers);
    }
}
