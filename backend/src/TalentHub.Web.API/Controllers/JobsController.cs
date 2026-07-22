using Microsoft.AspNetCore.Mvc;
using TalentHub.Integration.Communication.Abstractions;
using TalentHub.Integration.Communication.Models;

namespace TalentHub.Web.API.Controllers;

public sealed class JobsController(IJobAggregationService jobAggregationService) : BaseController
{
    [HttpGet]
    public async Task<ActionResult<JobAggregationResult>> Get(CancellationToken cancellationToken)
    {
        var result = await jobAggregationService.GetJobsAsync(cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }
}
