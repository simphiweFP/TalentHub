using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TalentHub.Integration.Greenhouse.Exceptions;
using TalentHub.Integration.Greenhouse.Models;
using TalentHub.Integration.Greenhouse.Services;

namespace TalentHub.Integration.Greenhouse.Endpoints;

public static class GreenhouseEndpoints
{
    public static IEndpointRouteBuilder MapGreenhouseEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/providers/greenhouse").WithTags("Greenhouse");

        group.MapGet("/jobs", async Task<IResult> (GreenhouseService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var jobs = await service.GetJobsAsync(cancellationToken).ConfigureAwait(false);
                return Results.Ok(jobs);
            }
            catch (GreenhouseException exception)
            {
                return Results.Problem(exception.Message, statusCode: StatusCodes.Status502BadGateway, title: exception.Code);
            }
        });

        group.MapGet("/jobs/search", async Task<IResult> (string? searchTerm, string? location, int pageNumber, int pageSize, GreenhouseService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var jobs = await service.SearchJobsAsync(new GreenhouseSearchRequest(searchTerm, location, pageNumber, pageSize), cancellationToken).ConfigureAwait(false);
                return Results.Ok(jobs);
            }
            catch (GreenhouseException exception)
            {
                return Results.Problem(exception.Message, statusCode: StatusCodes.Status502BadGateway, title: exception.Code);
            }
        });

        group.MapGet("/company/{externalCompanyId}", async Task<IResult> (string externalCompanyId, GreenhouseService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var companyJob = await service.GetCompanyJobPreviewAsync(externalCompanyId, cancellationToken).ConfigureAwait(false);
                return companyJob is null ? Results.NotFound() : Results.Ok(companyJob);
            }
            catch (GreenhouseException exception)
            {
                return Results.Problem(exception.Message, statusCode: StatusCodes.Status502BadGateway, title: exception.Code);
            }
        });

        return endpoints;
    }
}
