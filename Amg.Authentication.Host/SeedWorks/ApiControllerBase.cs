using System.Linq;
using Amg.Authentication.Host.Filters;
using Amg.Authentication.Shared;
using Amg.Authentication.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Amg.Authentication.Host.SeedWorks
{
    [ApiController]
    [PermissionAuthorize]
    public abstract class ApiControllerBase : ControllerBase
    {


        private UserInfo _userInfo;

        protected UserInfo UserInfo => _userInfo ??= User?.Claims?.ToList().ToUserInfo();

        protected string AccessToken => Request.Headers.Authorization;


        [NonAction]
        protected IActionResult OkResult(string message = "عملیات با موفقیت انجام شد")
        {
            return Ok(new ResponseMessage(message));
        }

        [NonAction]
        protected IActionResult OkResult<T>(T content, string message = "عملیات با موفقیت انجام شد")
        {
            return Ok(new ResponseMessage<T>(message, content));
        }

        [NonAction]
        protected IActionResult NotFoundResult(string message)
        {
            return NotFound(new ResponseMessage(message));
        }

        [NonAction]
        protected IActionResult NotFoundResult<T>(T content, string message)
        {
            return NotFound(new ResponseMessage<T>(message, content));
        }

        [NonAction]
        protected IActionResult BadRequestResult(string message)
        {
            return BadRequest(new ResponseMessage(message));
        }

        [NonAction]
        protected IActionResult BadRequestResult<T>(T content, string message)
        {
            return BadRequest(new ResponseMessage<T>(message, content));
        }
    }
}
