namespace TalentHub.Integration.Sql.Constants;

public static class IndexNames
{
    public const string IX_Users_UserName = "IX_Users_UserName";
    public const string IX_Users_Email = "IX_Users_Email";
    public const string IX_Companies_Name = "IX_Companies_Name";
    public const string IX_Jobs_CompanyId = "IX_Jobs_CompanyId";
    public const string IX_Jobs_CategoryId = "IX_Jobs_CategoryId";
    public const string IX_Jobs_IsActive = "IX_Jobs_IsActive";
    public const string IX_JobProviders_JobId_ProviderId = "IX_JobProviders_JobId_ProviderId";
    public const string IX_SavedJobs_UserId_JobId = "IX_SavedJobs_UserId_JobId";
    public const string IX_Skills_Name = "IX_Skills_Name";
    public const string IX_Locations_City = "IX_Locations_City";
}
