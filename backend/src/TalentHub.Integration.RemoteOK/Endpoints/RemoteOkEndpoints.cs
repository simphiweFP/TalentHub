using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TalentHub.Integration.RemoteOK.Exceptions;
using TalentHub.Integration.RemoteOK.Models;
using TalentHub.Integration.RemoteOK.Services;

namespace TalentHub.Integration.RemoteOK.Endpoints;

public static class RemoteOkEndpoints
{
    public static IEndpointRouteBuilder MapRemoteOkEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var group = endpoints.MapGroup("/api/providers/remoteok").WithTags("RemoteOK");

        group.MapGet("/jobs", async (RemoteOkService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var jobs = await service.GetJobsAsync(cancellationToken).ConfigureAwait(false);
                return Results.Ok(jobs);
            }
            catch (RemoteOkException exception)
            {
                return Results.Problem(exception.Message, statusCode: StatusCodes.Status502BadGateway, title: exception.Code);
            }
        });

        group.MapGet("/jobs/search", async (string? searchTerm, string? location, int pageNumber, int pageSize, RemoteOkService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var jobs = await service.SearchJobsAsync(new RemoteOkSearchRequest(searchTerm, location, pageNumber, pageSize), cancellationToken).ConfigureAwait(false);
                return Results.Ok(jobs);
            }
            catch (RemoteOkException exception)
            {
                return Results.Problem(exception.Message, statusCode: StatusCodes.Status502BadGateway, title: exception.Code);
            }
        });

        group.MapGet("/company/{externalCompanyId}", async (string externalCompanyId, RemoteOkService service, CancellationToken cancellationToken) =>
        {
            try
            {
                var companyJob = await service.GetCompanyJobPreviewAsync(externalCompanyId, cancellationToken).ConfigureAwait(false);
                return companyJob is null ? Results.NotFound() : Results.Ok(companyJob);
            }
            catch (RemoteOkException exception)
            {
                return Results.Problem(exception.Message, statusCode: StatusCodes.Status502BadGateway, title: exception.Code);
            }
        });

        return endpoints;
    }
}
