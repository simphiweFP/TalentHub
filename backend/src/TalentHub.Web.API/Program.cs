using TalentHub.Application;
using TalentHub.Infrastructure;
using TalentHub.Web.API.Extensions;
using TalentHub.Web.API.Middleware;
using TalentHub.Web.API.Options;
using TalentHub.Web.Main;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var swaggerOptions = builder.Configuration.GetSection(SwaggerOptions.SectionName).Get<SwaggerOptions>() ?? new SwaggerOptions();

builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext());

builder.Services.AddBackendFoundation(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddWebMain();

var app = builder.Build();

app.UseSerilogRequestLogging();
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseRateLimiter();

if (app.Environment.IsDevelopment())
{
    if (swaggerOptions.Enabled)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.RoutePrefix = swaggerOptions.RoutePrefix;
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "TalentHub API v1");
        });
    }
}

app.MapHealthChecks("/health");
app.MapControllers();

app.Run();
