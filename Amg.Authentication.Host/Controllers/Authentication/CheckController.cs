using System.Threading.Tasks;
using Amg.Authentication.Application.Contract;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Host.SeedWorks;
using Microsoft.AspNetCore.Mvc;

namespace Amg.Authentication.Host.Controllers.Authentication
{
    [Route(Constants.ApiPrefix + "/v1/[controller]")]
    public class CheckController : ApiControllerBase
    {
        private readonly ISignInService _signInService;

        public CheckController(ISignInService signInService)
        {
            _signInService = signInService;
        }

        [HttpGet("user")]
        public IActionResult GetUserInfo()
        {
            if (UserInfo == null)
                return Unauthorized();

            return OkResult(UserInfo);
        }

        [HttpGet("token")]
        public async Task<IActionResult> IsTokenValid()
        {
            if (UserInfo == null)
                return OkResult(false);

            var result = await _signInService.IsTokenValid(UserInfo.TokenId, UserInfo.UserId);
            return OkResult(result);
        }

    }
}
