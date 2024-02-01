using System.Linq;
using Amg.Authentication.Shared.Attributes;
using Amg.Authentication.Shared.Enums;
using Amg.Authentication.Shared.Permissions;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using SampleWebApp.Authentication;

namespace SampleWebApp.Controllers
{
    [ApiController]
    [PermissionAuthorize]
    [Route("api/[controller]")]
    public class RequestController : ControllerBase
    {
        [HttpGet("info")]
        [CheckPermission(RoleType.SystemUser, SamplePermissions.ViewRequestInfo)]
        public IActionResult Get()
        {
            return Ok(new
            {
                RequestIp = HttpContext.Connection.RemoteIpAddress.ToString(),
                Route = Request.GetDisplayUrl(),
                Headers = Request.Headers.ToDictionary(i => i.Key, i => i.Value),
            });
        }
    }
}
