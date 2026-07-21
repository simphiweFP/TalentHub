using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using TalentHub.Web.API.Options;
using TalentHub.Web.API.Security;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TalentHub.Web.API.Extensions;

public sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IOptions<ApiOptions> apiOptions, IOptions<AuthenticationOptions> authenticationOptions)
    : IConfigureOptions<SwaggerGenOptions>
{
    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = apiOptions.Value.Name,
                Version = description.ApiVersion.ToString(),
                Description = apiOptions.Value.Description
            });
        }

        options.AddSecurityDefinition(AuthenticationSchemeNames.JwtBearer, new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "JWT Bearer token using the Authorization header."
        });

        options.AddSecurityDefinition(AuthenticationSchemeNames.ApiKey, new OpenApiSecurityScheme
        {
            Name = authenticationOptions.Value.ApiKey.HeaderName,
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Description = "API key authentication using a custom header."
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = AuthenticationSchemeNames.JwtBearer
                    }
                },
                Array.Empty<string>()
            },
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = AuthenticationSchemeNames.ApiKey
                    }
                },
                Array.Empty<string>()
            }
        });
    }
}
