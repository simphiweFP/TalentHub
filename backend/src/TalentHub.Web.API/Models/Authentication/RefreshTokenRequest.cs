namespace TalentHub.Web.API.Models.Authentication;

public sealed record RefreshTokenRequest(string AccessToken, string RefreshToken);
