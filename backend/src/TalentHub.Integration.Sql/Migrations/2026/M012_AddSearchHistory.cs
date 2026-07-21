using System.Data;
using FluentMigrator;
using TalentHub.Integration.Sql.Constants;

namespace TalentHub.Integration.Sql.Migrations._2026;

/// <summary>
/// Creates the SearchHistory table to capture user search activity.
/// Dependencies: Users should exist if searches are linked to an account.
/// Rollback: safe after search analytics are no longer required.
/// </summary>
[Migration(202607221445, "Create SearchHistory table")]
public sealed class M012_AddSearchHistory : Migration
{
    public override void Up()
    {
        Create.Table(TableNames.SearchHistory)
            .WithColumn(ColumnNames.Id).AsGuid().NotNullable().PrimaryKey(ConstraintNames.PK_SearchHistory)
            .WithColumn(ColumnNames.UserId).AsGuid().Nullable()
            .WithColumn("SearchText").AsString(500).NotNullable()
            .WithColumn("LocationQuery").AsString(250).Nullable()
            .WithColumn("MinSalary").AsDecimal(18, 2).Nullable()
            .WithColumn("MaxSalary").AsDecimal(18, 2).Nullable()
            .WithColumn("FiltersJson").AsString(int.MaxValue).Nullable()
            .WithColumn("SearchedAtUtc").AsDateTime2().NotNullable();

        Create.ForeignKey("FK_SearchHistory_Users")
            .FromTable(TableNames.SearchHistory).ForeignColumn(ColumnNames.UserId)
            .ToTable(TableNames.Users).PrimaryColumn(ColumnNames.Id)
            .OnDelete(Rule.SetNull);

        Create.Index("IX_SearchHistory_UserId")
            .OnTable(TableNames.SearchHistory)
            .OnColumn(ColumnNames.UserId).Ascending()
            .WithOptions().NonClustered();
    }

    public override void Down()
    {
        Delete.Table(TableNames.SearchHistory);
    }
}
