using Microsoft.AspNetCore.Mvc;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Models;
using TalentHub.Web.API.Controllers;
using TalentHub.Web.API.Models.Authentication;

namespace TalentHub.Tests.Controllers;

public sealed class ControllerTests
{
    [Fact]
    public async Task AuthController_login_register_and_refresh_return_tokens()
    {
        // Arrange
        var authenticationService = new RecordingAuthenticationService();
        var controller = new AuthController(authenticationService);

        // Act
        var loginResult = await controller.Login(new LoginRequest("demo", "password"), CancellationToken.None).ConfigureAwait(false);
        var registerResult = await controller.Register(new RegisterRequest("demo", "demo@talenthub.local", "password"), CancellationToken.None).ConfigureAwait(false);
        var refreshResult = await controller.Refresh(new RefreshTokenRequest("access", "refresh"), CancellationToken.None).ConfigureAwait(false);

        // Assert
        Assert.IsType<OkObjectResult>(loginResult.Result);
        Assert.IsType<OkObjectResult>(registerResult.Result);
        Assert.IsType<OkObjectResult>(refreshResult.Result);
        Assert.Equal(1, authenticationService.LoginCalls);
        Assert.Equal(1, authenticationService.RegisterCalls);
        Assert.Equal(1, authenticationService.RefreshCalls);
    }

    [Fact]
    public async Task JobsController_returns_aggregated_jobs_result()
    {
        // Arrange
        var expected = new JobAggregationResult
        {
            Jobs = [new Job { Source = "openai", ExternalId = "1", Title = "Engineer", CompanyName = "Acme", PublishedAtUtc = DateTimeOffset.UtcNow }]
        };
        var aggregationService = new RecordingJobAggregationService(expected);
        var controller = new JobsController(aggregationService);

        // Act
        var result = await controller.Get(CancellationToken.None).ConfigureAwait(false);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        Assert.Same(expected, ok.Value);
        Assert.Equal(1, aggregationService.CallCount);
    }

    [Fact]
    public void HealthController_returns_healthy_payload()
    {
        // Arrange
        var controller = new HealthController();

        // Act
        var result = controller.Get();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        dynamic payload = ok.Value!;
        Assert.Equal("Healthy", (string)payload.status);
        Assert.Equal("TalentHub API", (string)payload.service);
    }

    private sealed class RecordingAuthenticationService : Web.API.Abstractions.IAuthenticationService
    {
        public int LoginCalls { get; private set; }
        public int RegisterCalls { get; private set; }
        public int RefreshCalls { get; private set; }

        public Task<TokenResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
        {
            LoginCalls++;
            return Task.FromResult(new TokenResponse("login-access", "login-refresh", DateTimeOffset.UtcNow));
        }

        public Task<TokenResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
        {
            RegisterCalls++;
            return Task.FromResult(new TokenResponse("register-access", "register-refresh", DateTimeOffset.UtcNow));
        }

        public Task<TokenResponse> RefreshAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
        {
            RefreshCalls++;
            return Task.FromResult(new TokenResponse("refresh-access", "refresh-refresh", DateTimeOffset.UtcNow));
        }
    }

    private sealed class RecordingJobAggregationService(JobAggregationResult result) : IJobAggregationService
    {
        public int CallCount { get; private set; }

        public Task<JobAggregationResult> GetJobsAsync(CancellationToken cancellationToken = default)
        {
            CallCount++;
            return Task.FromResult(result);
        }
    }
}
