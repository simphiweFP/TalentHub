namespace TalentHub.Integration.Sql.Views;

public static class ActiveJobsViewSql
{
    public const string Create = """
        CREATE VIEW [dbo].[vw_ActiveJobs]
        AS
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
            j.[IsActive],
            c.[Name] AS [CompanyName],
            c.[Slug] AS [CompanySlug],
            jc.[Name] AS [JobCategoryName],
            l.[City] AS [LocationCity],
            l.[Country] AS [LocationCountry],
            j.[CreatedAtUtc],
            j.[UpdatedAtUtc]
        FROM [dbo].[Jobs] AS j
        INNER JOIN [dbo].[Companies] AS c ON c.[Id] = j.[CompanyId]
        INNER JOIN [dbo].[JobCategories] AS jc ON jc.[Id] = j.[JobCategoryId]
        LEFT JOIN [dbo].[Locations] AS l ON l.[Id] = j.[LocationId]
        WHERE j.[IsActive] = 1;
        """;
}
