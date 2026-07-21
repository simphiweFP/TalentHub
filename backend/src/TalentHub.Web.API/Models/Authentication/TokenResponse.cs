namespace TalentHub.Web.API.Models.Authentication;

public sealed record TokenResponse(string AccessToken, string RefreshToken, DateTimeOffset ExpiresAt);
