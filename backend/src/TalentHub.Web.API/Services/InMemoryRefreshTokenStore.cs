using System.Collections.Concurrent;
using TalentHub.Web.API.Abstractions;
using TalentHub.Web.API.Identity;
using TalentHub.Web.API.Options;
using Microsoft.Extensions.Options;

namespace TalentHub.Web.API.Services;

public sealed class InMemoryRefreshTokenStore(IOptions<AuthenticationOptions> options) : IRefreshTokenStore
{
    private readonly AuthenticationOptions _options = options.Value;
    private readonly ConcurrentDictionary<string, RefreshTokenRecord> _refreshTokens = new();

    public Task<RefreshTokenRecord> IssueAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        var record = new RefreshTokenRecord(token, userId, DateTimeOffset.UtcNow.AddDays(_options.RefreshTokens.ExpirationDays));
        _refreshTokens[token] = record;
        return Task.FromResult(record);
    }

    public Task<bool> ValidateAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            _refreshTokens.TryGetValue(refreshToken, out var record)
            && !record.Revoked
            && record.ExpiresAt > DateTimeOffset.UtcNow);
    }

    public Task RevokeAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (_refreshTokens.TryGetValue(refreshToken, out var record))
        {
            _refreshTokens[refreshToken] = record with { Revoked = true };
        }

        return Task.CompletedTask;
    }
}
