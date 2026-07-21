namespace TalentHub.Web.API.Options;

public sealed class RefreshTokenOptions
{
    public int ExpirationDays { get; set; } = 7;

    public bool StoreInMemory { get; set; } = true;
}
