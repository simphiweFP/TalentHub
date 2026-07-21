using System.Data;
using FluentMigrator;
using TalentHub.Integration.Sql.Constants;

namespace TalentHub.Integration.Sql.Migrations._2026;

/// <summary>
/// Creates the JobProviders bridge table for external source tracking.
/// Dependencies: Jobs and Providers must exist before the relationships are used.
/// Rollback: safe after imported job records are removed.
/// </summary>
[Migration(202607221400, "Create JobProviders table")]
public sealed class M009_AddJobProvider : Migration
{
    public override void Up()
    {
        Create.Table(TableNames.JobProviders)
            .WithColumn(ColumnNames.JobId).AsGuid().NotNullable()
            .WithColumn(ColumnNames.ProviderId).AsInt32().NotNullable()
            .WithColumn("ExternalJobId").AsString(200).NotNullable()
            .WithColumn("ExternalUrl").AsString(500).Nullable()
            .WithColumn("ImportedAtUtc").AsDateTime2().NotNullable()
            .WithColumn(ColumnNames.IsActive).AsBoolean().NotNullable().WithDefaultValue(true);

        Create.PrimaryKey(ConstraintNames.PK_JobProviders)
            .OnTable(TableNames.JobProviders)
            .Columns(ColumnNames.JobId, ColumnNames.ProviderId, "ExternalJobId");

        Create.ForeignKey("FK_JobProviders_Jobs")
            .FromTable(TableNames.JobProviders).ForeignColumn(ColumnNames.JobId)
            .ToTable(TableNames.Jobs).PrimaryColumn(ColumnNames.Id)
            .OnDelete(Rule.Cascade);

        Create.ForeignKey("FK_JobProviders_Providers")
            .FromTable(TableNames.JobProviders).ForeignColumn(ColumnNames.ProviderId)
            .ToTable(TableNames.Providers).PrimaryColumn(ColumnNames.Id)
            .OnDelete(Rule.Cascade);

        Create.Index(IndexNames.IX_JobProviders_JobId_ProviderId)
            .OnTable(TableNames.JobProviders)
            .OnColumn(ColumnNames.JobId).Ascending()
            .OnColumn(ColumnNames.ProviderId).Ascending()
            .WithOptions().NonClustered();
    }

    public override void Down()
    {
        Delete.Table(TableNames.JobProviders);
    }
}
