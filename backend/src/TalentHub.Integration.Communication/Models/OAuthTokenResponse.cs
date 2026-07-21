namespace TalentHub.Integration.Communication.Models;

public sealed record OAuthTokenResponse(string AccessToken, DateTimeOffset ExpiresAt, string? TokenType = "Bearer", string? Scope = null);
