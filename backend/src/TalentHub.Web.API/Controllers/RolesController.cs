using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalentHub.Web.API.Models.Authentication;
using TalentHub.Web.API.Security;

namespace TalentHub.Web.API.Controllers;

[Authorize(Policy = AuthorizationPolicies.RequireAuthenticatedUser)]
public sealed class RolesController : BaseController
{
    [HttpGet]
    public ActionResult<IReadOnlyList<RoleResponse>> GetRoles() => Ok(new[]
    {
        new RoleResponse(Guid.NewGuid(), "Administrator", "Placeholder administrator role"),
        new RoleResponse(Guid.NewGuid(), "Manager", "Placeholder manager role"),
        new RoleResponse(Guid.NewGuid(), "User", "Placeholder user role")
    });
}
