using System;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract;
using Amg.Authentication.Application.Contract.Requests;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Host.Extensions;
using Amg.Authentication.Host.SeedWorks;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.Infrastructure.Enums;
using Amg.Authentication.Infrastructure.Extensions;
using Amg.Authentication.Infrastructure.Settings;
using Amg.Authentication.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SignInResult = Amg.Authentication.Application.Contract.Dtos.SignInResult;

namespace Amg.Authentication.Host.Controllers.Authentication
{
    [Route(Constants.ApiPrefix + "/v1/[controller]")]
    public class SignInController : ApiControllerBase
    {
        private readonly ICommandValidator _commandValidator;
        private readonly ISignInService _signInService;
        private readonly JwtTokenSettings _jwtTokenSettings;
        private readonly AuthSettings _authSettings;
        private readonly ICommandBus _commandBus;

        public SignInController(ICommandValidator commandValidator,
            ISignInService signInService,
            ICommandBus commandBus,
            IOptions<JwtTokenSettings> jwtTokenSettings,
            IOptions<AuthSettings> authSettings)
        {
            _commandValidator = commandValidator;
            _signInService = signInService;
            _authSettings = authSettings.Value;
            _jwtTokenSettings = jwtTokenSettings.Value;
            _commandBus = commandBus;
        }



        [HttpPost("password")]
        [AllowAnonymous]
        public async Task<IActionResult> SignInByPassword(SignInByPasswordRequest command)
        {
            _commandValidator.Validate(command);
            var loginResult = await _signInService.PasswordSignIn(command);

            if (!loginResult.IsSuccess)
                return BadRequestResult(loginResult.ToDto(), loginResult.Result.GetResultMessage());

            Response.Headers.Append("Authorization", "Bearer " + loginResult.AccessToken);
            //SetTokenCookieIfNeeded(loginResult);
            return OkResult(loginResult.ToDto(), loginResult.Result.GetResultMessage());
        }

        [HttpPost(nameof(SubmitRegisterWithPhoneNumber))]
        [AllowAnonymous]
        public async Task<IActionResult> SubmitRegisterWithPhoneNumber(SignInByPhoneNumberRequest command)
        {
            command.Validate();
            var loginResult = await _signInService.PhoneNumberSignIn(command);

            if (!loginResult.IsSuccess)
                return BadRequestResult(loginResult.ToDto(), loginResult.Result.GetResultMessage());

            Response.Headers.Append("Authorization", "Bearer " + loginResult.AccessToken);
            //SetTokenCookieIfNeeded(loginResult);
            return OkResult(loginResult.ToDto(), loginResult.Result.GetResultMessage());
        }


        [HttpPost("two-factor")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginByTwoFactorCode(SignInByTwoFactorCodeRequest command)
        {
            _commandValidator.Validate(command);
            var loginResult = await _signInService.TwoFactorSignIn(command);

            if (!loginResult.IsSuccess)
                return BadRequestResult(loginResult.ToDto(), loginResult.Result.GetResultMessage());

            Response.Headers.Append("Authorization", "Bearer " + loginResult.AccessToken);
            //SetTokenCookieIfNeeded(loginResult);
            return OkResult(loginResult.ToDto(), loginResult.Result.GetResultMessage());

        }


        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshSignIn()
        {
            var loginResult = await _signInService.RefreshSignIn();

            if (!loginResult.IsSuccess)
                return BadRequestResult(loginResult.ToDto(), loginResult.Result.GetResultMessage());

            Response.Headers.Append("Authorization", "Bearer " + loginResult.AccessToken);
            //SetTokenCookieIfNeeded(loginResult);
            return OkResult(loginResult.ToDto(), loginResult.Result.GetResultMessage());
        }


        #region PrivateMethods

        private void SetTokenCookieIfNeeded(SignInResult signInResult)
        {
            if (signInResult.Result == SignInResultType.LoginSuccessful ||
                signInResult.Result == SignInResultType.InvalidCredentials)
            {
                Response.Cookies.Delete(AuthConstants.CookieName, new CookieOptions()
                {
                    Domain = _authSettings.Token.Domain,
                    HttpOnly = true,
                    IsEssential = true,
                    Path = "/",
                    SameSite = SameSiteMode.Lax,
                    Secure = _authSettings.Token.IsSecure,
                });
            }

            if (signInResult.Result == SignInResultType.LoginSuccessful)
            {
                Response.Cookies.Append(AuthConstants.CookieName, signInResult.AccessToken, new CookieOptions()
                {
                    Domain = _authSettings.Token.Domain,
                    MaxAge = signInResult.Ticket.RefreshExpireAt - DateTime.Now,
                    // a.ammari : Expires is DEPRECATED,
                    HttpOnly = true,
                    IsEssential = true,
                    Path = "/",
                    // a.ammari: due to new SameSite Policy changes, SameSiteMode.None requires to be Secure
                    SameSite = SameSiteMode.Lax,
                    Secure = _authSettings.Token.IsSecure,
                });
            }
        }

        #endregion
    }
}
