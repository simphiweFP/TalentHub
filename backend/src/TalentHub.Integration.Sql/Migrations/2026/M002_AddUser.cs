using FluentMigrator;
using TalentHub.Integration.Sql.Constants;

namespace TalentHub.Integration.Sql.Migrations._2026;

/// <summary>
/// Creates the Users table used by the domain model and future identity integration.
/// Dependencies: none.
/// Rollback: safe once related profiles, jobs, saved jobs, and logs are removed.
/// </summary>
[Migration(202607221215, "Create Users table")]
public sealed class M002_AddUser : Migration
{
    public override void Up()
    {
        Create.Table(TableNames.Users)
            .WithColumn(ColumnNames.Id).AsGuid().NotNullable().PrimaryKey(ConstraintNames.PK_Users)
            .WithColumn("UserName").AsString(100).NotNullable()
            .WithColumn("NormalizedUserName").AsString(100).NotNullable()
            .WithColumn("Email").AsString(256).NotNullable()
            .WithColumn("NormalizedEmail").AsString(256).NotNullable()
            .WithColumn("PasswordHash").AsString(500).Nullable()
            .WithColumn("FirstName").AsString(100).Nullable()
            .WithColumn("LastName").AsString(100).Nullable()
            .WithColumn("DisplayName").AsString(200).Nullable()
            .WithColumn("PhoneNumber").AsString(30).Nullable()
            .WithColumn(ColumnNames.IsActive).AsBoolean().NotNullable().WithDefaultValue(true)
            .WithColumn("IsDeleted").AsBoolean().NotNullable().WithDefaultValue(false)
            .WithColumn(ColumnNames.CreatedAtUtc).AsDateTime2().NotNullable()
            .WithColumn(ColumnNames.UpdatedAtUtc).AsDateTime2().Nullable();

        Create.Index(IndexNames.IX_Users_UserName)
            .OnTable(TableNames.Users)
            .OnColumn("NormalizedUserName").Ascending()
            .WithOptions().Unique();

        Create.Index(IndexNames.IX_Users_Email)
            .OnTable(TableNames.Users)
            .OnColumn("NormalizedEmail").Ascending()
            .WithOptions().Unique();
    }

    public override void Down()
    {
        Delete.Table(TableNames.Users);
    }
}
