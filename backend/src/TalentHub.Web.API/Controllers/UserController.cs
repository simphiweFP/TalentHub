using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentHub.Web.API.Models.Authentication;
using TalentHub.Web.API.Security;

namespace TalentHub.Web.API.Controllers;

[Authorize(Policy = AuthorizationPolicies.RequireAuthenticatedUser)]
public sealed class UserController : BaseController
{
    [HttpGet("me")]
    public ActionResult<UserResponse> GetCurrentUser() => Ok(new UserResponse(
        Guid.NewGuid(),
        "placeholder.user",
        "placeholder.user@talenthub.local",
        ["User"],
        ["user.read"]));
}
