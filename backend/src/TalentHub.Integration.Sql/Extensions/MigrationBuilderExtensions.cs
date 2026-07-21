using FluentMigrator;
using FluentMigrator.Builders.Create.Table;

namespace TalentHub.Integration.Sql.Extensions;

public static class MigrationBuilderExtensions
{
    public static ICreateTableColumnOptionOrWithColumnSyntax AddAuditColumns(this ICreateTableWithColumnSyntax table)
    {
        return table
            .WithColumn("CreatedAtUtc").AsDateTime2().NotNullable()
            .WithColumn("UpdatedAtUtc").AsDateTime2().Nullable();
    }

    public static ICreateTableColumnOptionOrWithColumnSyntax AddStandardFlags(this ICreateTableWithColumnSyntax table)
    {
        return table
            .WithColumn("IsActive").AsBoolean().NotNullable().WithDefaultValue(true);
    }
}
