using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TalentHub.Integration.Communication;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.OpenAI;
using TalentHub.Integration.OpenAI.Providers;
using TalentHub.Integration.Sql;
using TalentHub.Integration.Sql.Abstractions;
using TalentHub.Integration.Communication.Registry;
using TalentHub.Web.API.Extensions;
using TalentHub.Web.API;
using TalentHub.Web.API.Abstractions;

namespace TalentHub.Tests.DependencyInjection;

public sealed class DependencyInjectionTests
{
    [Fact]
    public void Service_registration_resolves_core_application_services()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Authentication:Jwt:Issuer"] = "TalentHub",
                ["Authentication:Jwt:Audience"] = "TalentHub",
                ["Authentication:Jwt:SigningKey"] = "change-me-change-me-change-me-change-me",
                ["Authentication:Jwt:AccessTokenMinutes"] = "60",
                ["Authentication:RefreshTokens:ExpirationDays"] = "7",
                ["Authentication:RefreshTokens:StoreInMemory"] = "true",
                ["Authentication:ApiKey:HeaderName"] = "X-API-Key",
                ["Authentication:ApiKey:Value"] = "change-me",
                ["Authentication:ApiKey:Enabled"] = "true",
                ["Sql:ConnectionString"] = "Server=(localdb)\\mssqllocaldb;Database=TalentHub.Tests;Trusted_Connection=True;TrustServerCertificate=True;"
            })
            .Build();

        // Act
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddBackendFoundation(configuration);
        services.AddCommunicationIntegration();
        services.AddOpenAiIntegration();
        services.AddSqlIntegration(configuration);
        services.AddWebApi();
        using var provider = services.BuildServiceProvider();

        // Assert
        Assert.NotNull(provider.GetRequiredService<IAuthenticationService>());
        Assert.NotNull(provider.GetRequiredService<IJwtTokenService>());
        Assert.NotNull(provider.GetRequiredService<IRefreshTokenStore>());
        Assert.NotNull(provider.GetRequiredService<IAIProvider>());
        Assert.NotNull(provider.GetRequiredService<IJobAggregationService>());
        Assert.NotNull(provider.GetRequiredService<ProviderRegistry>());
        Assert.NotNull(provider.GetRequiredService<ICommandExecutor>());
    }
}
