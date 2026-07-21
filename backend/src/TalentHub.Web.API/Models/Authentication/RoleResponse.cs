namespace TalentHub.Web.API.Models.Authentication;

public sealed record RoleResponse(Guid Id, string Name, string? Description = null);
