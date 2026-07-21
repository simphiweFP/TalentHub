using System.Data;
using FluentMigrator;
using TalentHub.Integration.Sql.Constants;

namespace TalentHub.Integration.Sql.Migrations._2026;

/// <summary>
/// Creates the AuditLogs table for traceability and future compliance needs.
/// Dependencies: Users may be referenced as the actor but are not required.
/// Rollback: safe after audit retention requirements are met.
/// </summary>
[Migration(202607221500, "Create AuditLogs table")]
public sealed class M013_AddAuditLog : Migration
{
    public override void Up()
    {
        Create.Table(TableNames.AuditLogs)
            .WithColumn(ColumnNames.Id).AsGuid().NotNullable().PrimaryKey(ConstraintNames.PK_AuditLogs)
            .WithColumn("ActorUserId").AsGuid().Nullable()
            .WithColumn("EntityName").AsString(150).NotNullable()
            .WithColumn("EntityId").AsString(150).NotNullable()
            .WithColumn("Action").AsString(100).NotNullable()
            .WithColumn("BeforeDataJson").AsString(int.MaxValue).Nullable()
            .WithColumn("AfterDataJson").AsString(int.MaxValue).Nullable()
            .WithColumn("CorrelationId").AsString(100).Nullable()
            .WithColumn("OccurredAtUtc").AsDateTime2().NotNullable();

        Create.ForeignKey("FK_AuditLogs_Users")
            .FromTable(TableNames.AuditLogs).ForeignColumn("ActorUserId")
            .ToTable(TableNames.Users).PrimaryColumn(ColumnNames.Id)
            .OnDelete(Rule.SetNull);

        Create.Index("IX_AuditLogs_EntityName_EntityId")
            .OnTable(TableNames.AuditLogs)
            .OnColumn("EntityName").Ascending()
            .OnColumn("EntityId").Ascending()
            .WithOptions().NonClustered();
    }

    public override void Down()
    {
        Delete.Table(TableNames.AuditLogs);
    }
}
