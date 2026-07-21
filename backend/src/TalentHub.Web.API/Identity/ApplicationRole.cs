namespace TalentHub.Web.API.Identity;

public sealed record ApplicationRole(Guid Id, string Name, string? Description = null);
