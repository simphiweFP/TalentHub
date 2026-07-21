namespace TalentHub.Web.API.Security;

public static class AuthorizationPolicies
{
    public const string RequireAuthenticatedUser = "RequireAuthenticatedUser";

    public const string RequireAdministratorRole = "RequireAdministratorRole";

    public const string RequireManagerRole = "RequireManagerRole";

    public const string RequireUserRole = "RequireUserRole";

    public const string RequireApiKey = "RequireApiKey";
}
