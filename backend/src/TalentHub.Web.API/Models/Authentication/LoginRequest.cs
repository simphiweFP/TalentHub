namespace TalentHub.Web.API.Models.Authentication;

public sealed record LoginRequest(string UserNameOrEmail, string Password);
