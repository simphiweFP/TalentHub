using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentHub.Web.API.Models.Authentication;
using TalentHub.Web.API.Security;

namespace TalentHub.Web.API.Controllers;

[Authorize(Policy = AuthorizationPolicies.RequireAuthenticatedUser)]
public sealed class ClaimsController : BaseController
{
    [HttpGet]
    public ActionResult<IReadOnlyList<ClaimResponse>> GetClaims() => Ok(new[]
    {
        new ClaimResponse(ClaimNames.UserId, Guid.NewGuid().ToString()),
        new ClaimResponse(ClaimNames.UserName, "placeholder.user"),
        new ClaimResponse(ClaimNames.Email, "placeholder.user@talenthub.local")
    });
}
