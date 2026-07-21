namespace TalentHub.Integration.Sql.Functions;

public static class NormalizeSearchTermSql
{
    public const string Create = """
        CREATE FUNCTION [dbo].[fn_NormalizeSearchTerm]
        (
            @SearchTerm NVARCHAR(500)
        )
        RETURNS NVARCHAR(500)
        AS
        BEGIN
            RETURN LOWER(LTRIM(RTRIM(REPLACE(ISNULL(@SearchTerm, N''), CHAR(9), N' '))));
        END;
        """;
}
