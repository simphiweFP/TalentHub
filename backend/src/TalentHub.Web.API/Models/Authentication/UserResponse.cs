namespace TalentHub.Web.API.Models.Authentication;

public sealed record UserResponse(Guid Id, string UserName, string Email, IReadOnlyCollection<string> Roles, IReadOnlyCollection<string> Claims);
