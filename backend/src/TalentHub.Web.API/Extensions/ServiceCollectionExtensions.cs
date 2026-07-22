using System.IO.Compression;
using System.Text;
using System.Threading.RateLimiting;
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Options;
using TalentHub.Integration.Communication.Services;
using TalentHub.Web.API.BackgroundWorkers;
using TalentHub.Web.API.Abstractions;
using TalentHub.Web.API.Authentication;
using TalentHub.Web.API.Options;
using TalentHub.Web.API.Security;
using TalentHub.Web.API.Services;
using Swashbuckle.AspNetCore.SwaggerGen;
using AuthOptions = TalentHub.Web.API.Options.AuthenticationOptions;
using AuthService = TalentHub.Web.API.Services.AuthenticationService;
using IAuthService = TalentHub.Web.API.Abstractions.IAuthenticationService;

namespace TalentHub.Web.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddBackendFoundation(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AuthOptions>(configuration.GetSection(AuthOptions.SectionName));
        services.Configure<ApiOptions>(configuration.GetSection(ApiOptions.SectionName));
        services.Configure<CacheOptions>(configuration.GetSection(CacheOptions.SectionName));
        services.Configure<BackgroundWorkerOptions>(configuration.GetSection(BackgroundWorkerOptions.SectionName));
        services.Configure<RateLimitingOptions>(configuration.GetSection(RateLimitingOptions.SectionName));
        services.Configure<SwaggerOptions>(configuration.GetSection(SwaggerOptions.SectionName));

        services.AddMemoryCache();
        services.AddDistributedMemoryCache();
        services.AddSingleton<ICacheKeyBuilder, CacheKeyBuilder>();
        services.AddSingleton<ICacheStore>(serviceProvider =>
        {
            var cacheOptions = serviceProvider.GetRequiredService<IOptions<CacheOptions>>().Value;
            return cacheOptions.Enabled && cacheOptions.UseDistributedCache
                ? new DistributedCacheStore(serviceProvider.GetRequiredService<IDistributedCache>(), serviceProvider.GetRequiredService<ICacheKeyBuilder>())
                : new MemoryCacheStore(serviceProvider.GetRequiredService<IMemoryCache>(), serviceProvider.GetRequiredService<ICacheKeyBuilder>());
        });

        var authenticationOptions = configuration.GetSection(AuthOptions.SectionName).Get<AuthOptions>() ?? new AuthOptions();

        services.AddSingleton<IJwtTokenService, JwtTokenService>();
        services.AddSingleton<IRefreshTokenStore, InMemoryRefreshTokenStore>();
        services.AddScoped<IAuthService, AuthService>();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateTokenReplay = false,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = authenticationOptions.Jwt.Issuer,
                    ValidAudience = authenticationOptions.Jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationOptions.Jwt.SigningKey))
                };
            })
            .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(AuthenticationSchemeNames.ApiKey, _ => { });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicies.RequireAuthenticatedUser, policy => policy.RequireAuthenticatedUser());
            options.AddPolicy(AuthorizationPolicies.RequireAdministratorRole, policy => policy.RequireRole("Administrator"));
            options.AddPolicy(AuthorizationPolicies.RequireManagerRole, policy => policy.RequireRole("Manager"));
            options.AddPolicy(AuthorizationPolicies.RequireUserRole, policy => policy.RequireRole("User"));
            options.AddPolicy(AuthorizationPolicies.RequireApiKey, policy =>
            {
                policy.AddAuthenticationSchemes(AuthenticationSchemeNames.ApiKey);
                policy.RequireAuthenticatedUser();
            });
        });

        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddHealthChecks()
            .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Healthy"));

        services.AddResponseCompression(options =>
        {
            options.EnableForHttps = true;
            options.Providers.Add<BrotliCompressionProvider>();
            options.Providers.Add<GzipCompressionProvider>();
            options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(["application/json"]);
        });

        services.Configure<BrotliCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        services.Configure<GzipCompressionProviderOptions>(options =>
        {
            options.Level = CompressionLevel.Fastest;
        });

        services.AddRateLimiter(options =>
        {
            var rateLimitOptions = configuration.GetSection(RateLimitingOptions.SectionName).Get<RateLimitingOptions>() ?? new RateLimitingOptions();

            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(_ =>
                RateLimitPartition.GetFixedWindowLimiter(
                    "global",
                    _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = rateLimitOptions.PermitLimit,
                        Window = TimeSpan.FromSeconds(rateLimitOptions.WindowSeconds),
                        QueueLimit = rateLimitOptions.QueueLimit,
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                    }));
        });

        services.AddSwaggerGen();
        services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        services.AddHostedService<JobSynchronizationWorker>();
        services.AddHostedService<CompanyRefreshWorker>();
        services.AddHostedService<ExpiredDataCleanupWorker>();

        return services;
    }
}
