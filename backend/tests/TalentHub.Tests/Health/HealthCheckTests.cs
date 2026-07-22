using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TalentHub.Web.API.Extensions;

namespace TalentHub.Tests.Health;

public sealed class HealthCheckTests
{
    [Fact]
    public async Task Health_checks_are_registered_and_report_healthy()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Authentication:Jwt:Issuer"] = "TalentHub",
            ["Authentication:Jwt:Audience"] = "TalentHub",
            ["Authentication:Jwt:SigningKey"] = "change-me-change-me-change-me-change-me",
            ["Authentication:Jwt:AccessTokenMinutes"] = "60",
            ["Authentication:RefreshTokens:ExpirationDays"] = "7",
            ["Authentication:RefreshTokens:StoreInMemory"] = "true",
            ["Authentication:ApiKey:HeaderName"] = "X-API-Key",
            ["Authentication:ApiKey:Value"] = "change-me",
            ["Authentication:ApiKey:Enabled"] = "true"
        }).Build();

        // Act
        services.AddLogging();
        services.AddBackendFoundation(configuration);
        using var provider = services.BuildServiceProvider();
        var healthService = provider.GetRequiredService<HealthCheckService>();
        var report = await healthService.CheckHealthAsync().ConfigureAwait(false);

        // Assert
        Assert.Equal(HealthStatus.Healthy, report.Status);
        Assert.Contains(report.Entries, entry => entry.Key == "self" && entry.Value.Status == HealthStatus.Healthy);
    }
}
