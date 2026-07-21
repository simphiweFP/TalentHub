using Microsoft.AspNetCore.Mvc;

namespace TalentHub.Web.API.Controllers;

public sealed class StatusController : BaseController
{
    [HttpGet]
    public IActionResult Get() => Ok(new
    {
        status = "Running",
        service = "TalentHub API"
    });
}
