using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TalentHub.Integration.Lever.Exceptions;
using TalentHub.Integration.Lever.Models;
using TalentHub.Integration.Lever.Services;

namespace TalentHub.Integration.Lever.Endpoints;

public static class LeverEndpoints
{
    public static IEndpointRouteBuilder MapLeverEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/providers/lever").WithTags("Lever");

        group.MapGet("/jobs", async Task<IResult> (LeverService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var jobs = await service.GetJobsAsync(cancellationToken).ConfigureAwait(false);
                return Results.Ok(jobs);
            }
            catch (LeverException exception)
            {
                return Results.Problem(exception.Message, statusCode: StatusCodes.Status502BadGateway, title: exception.Code);
            }
        });

        group.MapGet("/jobs/search", async Task<IResult> (string? searchTerm, string? location, int pageNumber, int pageSize, LeverService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var jobs = await service.SearchJobsAsync(new LeverSearchRequest(searchTerm, location, pageNumber, pageSize), cancellationToken).ConfigureAwait(false);
                return Results.Ok(jobs);
            }
            catch (LeverException exception)
            {
                return Results.Problem(exception.Message, statusCode: StatusCodes.Status502BadGateway, title: exception.Code);
            }
        });

        group.MapGet("/company/{externalCompanyId}", async Task<IResult> (string externalCompanyId, LeverService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var companyJob = await service.GetCompanyJobPreviewAsync(externalCompanyId, cancellationToken).ConfigureAwait(false);
                return companyJob is null ? Results.NotFound() : Results.Ok(companyJob);
            }
            catch (LeverException exception)
            {
                return Results.Problem(exception.Message, statusCode: StatusCodes.Status502BadGateway, title: exception.Code);
            }
        });

        return endpoints;
    }
}
