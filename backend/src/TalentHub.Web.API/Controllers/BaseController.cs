using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace TalentHub.Web.API.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseController : ControllerBase
{
}
