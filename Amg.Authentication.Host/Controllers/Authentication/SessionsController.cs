using System;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Host.SeedWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amg.Authentication.Host.Controllers.Authentication
{
    [Route(Constants.ApiPrefix + "/v1/[controller]")]
    public class SessionsController : ApiControllerBase
    {
        private readonly ISignInService _signInService;

        public SessionsController(ISignInService signInService)
        {
            _signInService = signInService;
        }


        [HttpGet("")]
        public async Task<IActionResult> GetActiveSessions()
        {
            if (UserInfo == null)
                return Ok(false);

            var result = await _signInService.GetActiveSessions(UserInfo.UserId);
            return OkResult(result);
        }

        // a.ammari: به دلیل این که کاربر بتواند قبل از ورود،
        // سشن های دیگر خود را ببندد از نوع 
        // anonymous
        // در نظر گرفته شده است
        [HttpPost("{tokenId}/end")]
        [AllowAnonymous]
        public async Task<IActionResult> EndSession(Guid tokenId)
        {
            await _signInService.SignOut(tokenId);
            return OkResult();
        }
    }
}
