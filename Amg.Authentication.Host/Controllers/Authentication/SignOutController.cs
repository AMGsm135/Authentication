using System.Threading.Tasks;
using Amg.Authentication.Application.Contract;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Host.SeedWorks;
using Amg.Authentication.Infrastructure.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Amg.Authentication.Host.Controllers.Authentication
{
    [Route(Constants.ApiPrefix + "/v1/[controller]")]
    public class SignOutController : ApiControllerBase
    {
        private readonly ISignInService _signInService;
        private readonly AuthSettings _authSettings;

        public SignOutController(ISignInService signInService, IOptions<AuthSettings> authSettings)
        {
            _signInService = signInService;
            _authSettings = authSettings.Value;
        }

        // a.ammari: به دلیل این که سشن کاربر ممکن است منقضی شده باشد،
        // برای این که کوکی کاربر نیز حذف شود از نوع
        // anonymous
        // در نظر گرفته شده است
        [HttpGet("")]
        [HttpPost("")]
        [AllowAnonymous]
        public async Task<IActionResult> SignOut()
        {
            await _signInService.SignOut();
            /*Response.Cookies.Delete(AuthConstants.CookieName, new CookieOptions()
            {
                Domain = _authSettings.Token.Domain,
                HttpOnly = true,
                IsEssential = true,
                Path = "/",
                SameSite = SameSiteMode.Lax,
                Secure = _authSettings.Token.IsSecure,
            }); */
            return OkResult();
        }

    }
}
