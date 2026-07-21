namespace TalentHub.Integration.Sql.StoredProcedures;

public static class SearchJobsSql
{
    public const string Create = """
        CREATE PROCEDURE [dbo].[usp_SearchJobs]
            @SearchTerm NVARCHAR(500),
            @CompanyId UNIQUEIDENTIFIER = NULL,
            @JobCategoryId INT = NULL,
            @PageNumber INT = 1,
            @PageSize INT = 25
        AS
        BEGIN
            SET NOCOUNT ON;

            SELECT
                j.[Id],
                j.[Title],
                j.[Slug],
                j.[Description],
                j.[EmploymentType],
                j.[WorkMode],
                j.[SeniorityLevel],
                j.[SalaryMin],
                j.[SalaryMax],
                j.[CurrencyCode],
                j.[IsRemote],
                c.[Name] AS [CompanyName],
                jc.[Name] AS [JobCategoryName]
            FROM [dbo].[Jobs] AS j
            INNER JOIN [dbo].[Companies] AS c ON c.[Id] = j.[CompanyId]
            INNER JOIN [dbo].[JobCategories] AS jc ON jc.[Id] = j.[JobCategoryId]
            WHERE j.[IsActive] = 1
              AND (@CompanyId IS NULL OR j.[CompanyId] = @CompanyId)
              AND (@JobCategoryId IS NULL OR j.[JobCategoryId] = @JobCategoryId)
              AND (dbo.[fn_NormalizeSearchTerm](@SearchTerm) = N'' OR dbo.[fn_NormalizeSearchTerm](j.[Title]) LIKE N'%' + dbo.[fn_NormalizeSearchTerm](@SearchTerm) + N'%')
            ORDER BY j.[CreatedAtUtc] DESC
            OFFSET (@PageNumber - 1) * @PageSize ROWS
            FETCH NEXT @PageSize ROWS ONLY;
        END;
        """;
}
