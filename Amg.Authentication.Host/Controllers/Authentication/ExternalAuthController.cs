using Amg.Authentication.Application.Contract;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.DomainModel.Modules.Users;
using Amg.Authentication.Host.SeedWorks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Constants = Amg.Authentication.Application.Contract.Constants;
using Amg.Authentication.Application.Contract.Requests;

namespace Amg.Authentication.Host.Controllers.Authentication
{
    [Route(Constants.ApiPrefix + "/v1/[controller]")]
    public class ExternalAuthController : ApiControllerBase
    {

        private readonly ISignInService _signInService;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;

        public ExternalAuthController(ISignInService signInService, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _signInService = signInService;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost(nameof(ExternalLoginAsync))]
        public async Task<IActionResult> ExternalLoginAsync(string provider, string returnUrl)
        {
            var redirecturl = "https://localhost:7502/signin-google";
            var clientID = "581147649015-rv5eidhtfq1qkosqvvbk17l9p9fs6u6c.apps.googleusercontent.com";
            string responseType = "code";
            var gooleAuth = $"https://accounts.google.com/o/outh2/v2/auth?scope=openid%20email&response_type=token&redirect_uri={redirecturl}&client_id={clientID}";
            string scope = "openid profile email";
            string state = "your-random-state-value";

            var googleAuthenticationUrl = "https://accounts.google.com/o/oauth2/v2/auth?" +
             "client_id=581147649015-rv5eidhtfq1qkosqvvbk17l9p9fs6u6c.apps.googleusercontent.com" + // Replace with your Google OAuth client ID
             "&redirect_uri=https://localhost:7502/signin-google" + // Replace with your redirect URI
             "&response_type=code" +
             "&scope=openid+profile+email";

            string googleAuthUrl = $"https://accounts.google.com/v3/signin/identifier" +
             $"?client_id={Uri.EscapeDataString(clientID)}" +
             $"&redirect_uri={Uri.EscapeDataString(redirecturl)}" +
             $"&response_type={responseType}" +
             $"&scope={Uri.EscapeDataString(scope)}" +
             $"&state={Uri.EscapeDataString(state)}";

            var redirectUrl = Url.Action("ExternalLoginCallback", "ExternalAuth",
            new { ReturnUrl = returnUrl });

            var properties =
                _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return new ChallengeResult(provider, properties);
        }

        [AllowAnonymous]
        [HttpPost(nameof(ExternalLoginCallback))]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            bool userWasCreatedOrNot = false;
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            //var user = await _userManager.FindByEmailAsync(userEmail);
            try
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();

                var authenticationResult = await HttpContext.AuthenticateAsync("Google");

                if (!authenticationResult.Succeeded)
                {
                    return BadRequest("Google authentication failed.");
                }

                var userInformation = new
                {
                    UserId = authenticationResult.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    Email = authenticationResult.Principal.FindFirst(ClaimTypes.Email)?.Value,
                    FirstName = authenticationResult.Principal.FindFirst(ClaimTypes.GivenName)?.Value,
                    LastName = authenticationResult.Principal.FindFirst(ClaimTypes.Surname)?.Value
                };

                var existingUser = await _userManager.FindByEmailAsync(userEmail);

                if (existingUser == null)
                {
                    userWasCreatedOrNot = true;
                    var newUser = new User(userInformation.Email, userInformation.FirstName, userInformation.LastName, Infrastructure.Enums.PersonType.Individual, "Google", null, null);

                    var createUserResult = await _userManager.CreateAsync(newUser);

                    if (!createUserResult.Succeeded)
                    {
                        return BadRequest("User creation failed. ");
                    }
                }

                SignInByExternalProviderRequest signInByExternalProviderRequest = new SignInByExternalProviderRequest();
                signInByExternalProviderRequest.Email = userEmail;
                var rtn = await _signInService.ExternalProviderSignIn(signInByExternalProviderRequest, userWasCreatedOrNot);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    authenticationResult.Principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = true, // You can customize this based on your needs.
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60) // Token expiration time.
                    });

                await _userManager.AddLoginAsync(await _userManager.FindByEmailAsync(userEmail), info);


                return Ok(new { Token = rtn.AccessToken, Message = "Successfully Login With ExternalProvider" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
