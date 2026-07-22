namespace TalentHub.Integration.Communication.Models;

public sealed record ProviderFailure(
    string ProviderName,
    string ErrorMessage,
    int AttemptCount);
