namespace TalentHub.Integration.Communication.Options;

public sealed record CacheEntryPolicy(TimeSpan AbsoluteExpirationRelativeToNow, TimeSpan? SlidingExpiration = null);
