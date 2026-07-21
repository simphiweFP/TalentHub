using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using TalentHub.Web.API.Options;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TalentHub.Web.API.Extensions;

public sealed class ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider, IOptions<ApiOptions> apiOptions)
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
    }
}
