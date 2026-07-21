namespace TalentHub.Web.API.Models.Authentication;

public sealed record RegisterRequest(string UserName, string Email, string Password, string? DisplayName = null);
