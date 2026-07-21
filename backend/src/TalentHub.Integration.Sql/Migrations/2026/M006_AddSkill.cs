using FluentMigrator;
using TalentHub.Integration.Sql.Constants;

namespace TalentHub.Integration.Sql.Migrations._2026;

/// <summary>
/// Creates the Skills table used for candidate and job skill matching.
/// Dependencies: none.
/// Rollback: safe when join data is removed.
/// </summary>
[Migration(202607221315, "Create Skills table")]
public sealed class M006_AddSkill : Migration
{
    public override void Up()
    {
        Create.Table(TableNames.Skills)
            .WithColumn(ColumnNames.Id).AsGuid().NotNullable().PrimaryKey(ConstraintNames.PK_Skills)
            .WithColumn(ColumnNames.Name).AsString(150).NotNullable()
            .WithColumn(ColumnNames.Slug).AsString(150).NotNullable()
            .WithColumn("Description").AsString(500).Nullable()
            .WithColumn(ColumnNames.IsActive).AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn(ColumnNames.CreatedAtUtc).AsDateTime2().NotNullable();

        Create.Index(IndexNames.IX_Skills_Name)
            .OnTable(TableNames.Skills)
            .OnColumn(ColumnNames.Name).Ascending()
            .WithOptions().Unique();

        Create.Index("IX_Skills_Slug")
            .OnTable(TableNames.Skills)
            .OnColumn(ColumnNames.Slug).Ascending()
            .WithOptions().Unique();
    }

    public override void Down()
    {
        Delete.Table(TableNames.Skills);
    }
}
