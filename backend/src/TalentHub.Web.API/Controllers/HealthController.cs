using Microsoft.AspNetCore.Mvc;

namespace TalentHub.Web.API.Controllers;

public sealed class HealthController : BaseController
{
    [HttpGet]
    public IActionResult Get() => Ok(new
    {
        status = "Healthy",
        service = "TalentHub API"
    });
}
