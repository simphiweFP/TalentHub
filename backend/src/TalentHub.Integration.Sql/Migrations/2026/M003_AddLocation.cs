using FluentMigrator;
using TalentHub.Integration.Sql.Constants;

namespace TalentHub.Integration.Sql.Migrations._2026;

/// <summary>
/// Creates the Locations table used for company, job, and candidate location data.
/// Dependencies: none.
/// Rollback: safe after dependent records are removed or reassigned.
/// </summary>
[Migration(202607221230, "Create Locations table")]
public sealed class M003_AddLocation : Migration
{
    public override void Up()
    {
        Create.Table(TableNames.Locations)
            .WithColumn(ColumnNames.Id).AsGuid().NotNullable().PrimaryKey(ConstraintNames.PK_Locations)
            .WithColumn("Country").AsString(100).NotNullable()
            .WithColumn("StateProvince").AsString(100).Nullable()
            .WithColumn("City").AsString(100).NotNullable()
            .WithColumn("PostalCode").AsString(20).Nullable()
            .WithColumn("AddressLine1").AsString(250).Nullable()
            .WithColumn("AddressLine2").AsString(250).Nullable()
            .WithColumn("Latitude").AsDecimal(18, 8).Nullable()
            .WithColumn("Longitude").AsDecimal(18, 8).Nullable()
            .WithColumn(ColumnNames.CreatedAtUtc).AsDateTime2().NotNullable();

        Create.Index(IndexNames.IX_Locations_City)
            .OnTable(TableNames.Locations)
            .OnColumn("City").Ascending()
            .WithOptions().NonClustered();
    }

    public override void Down()
    {
        Delete.Table(TableNames.Locations);
    }
}
