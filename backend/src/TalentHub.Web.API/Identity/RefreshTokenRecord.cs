namespace TalentHub.Web.API.Identity;

public sealed record RefreshTokenRecord(string Token, Guid UserId, DateTimeOffset ExpiresAt, bool Revoked = false);
