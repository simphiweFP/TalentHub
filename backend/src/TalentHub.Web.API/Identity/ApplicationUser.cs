namespace TalentHub.Web.API.Identity;

public sealed record ApplicationUser(Guid Id, string UserName, string Email, string? DisplayName = null);
