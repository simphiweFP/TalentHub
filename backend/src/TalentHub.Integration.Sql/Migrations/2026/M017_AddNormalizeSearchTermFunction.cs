using FluentMigrator;
using TalentHub.Integration.Sql.Functions;

namespace TalentHub.Integration.Sql.Migrations._2026;

/// <summary>
/// Creates the normalize search term function used by the search stored procedure.
/// Dependencies: none beyond the dbo schema.
/// Rollback: safe because the procedure can be recreated after removal.
/// </summary>
[Migration(202607221600, "Create normalize search term function")]
public sealed class M017_AddNormalizeSearchTermFunction : Migration
{
    public override void Up() => Execute.Sql(NormalizeSearchTermSql.Create);

    public override void Down() => Execute.Sql("DROP FUNCTION IF EXISTS [dbo].[fn_NormalizeSearchTerm];");
}
