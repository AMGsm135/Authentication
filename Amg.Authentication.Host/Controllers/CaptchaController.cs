using Amg.Authentication.Application.Contract;
using Amg.Authentication.Application.Contract.Dtos;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Host.SeedWorks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amg.Authentication.Host.Controllers
{
    [Route(Constants.ApiPrefix + "/v1/[controller]")]
    [AllowAnonymous]
    public class CaptchaController : ApiControllerBase
    {
        private readonly ICaptchaManager _captchaManager;

        public CaptchaController(ICaptchaManager captchaManager)
        {
            _captchaManager = captchaManager;
        }

        /// <summary>
        /// get captcha
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetCaptcha")]
        public IActionResult Get()
        {
            var captcha = _captchaManager.GenerateCaptcha();
            return OkResult(captcha);
        }

        /// <summary>
        /// get id to remove captcha and resend new captcha
        /// </summary>
        /// <remarks> remark </remarks>
        /// <param name="captchaId" example = "d99b189a-905d-4ef6-afd1-0b27d0013ffe">id of captcha to remove</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetCaptcha/{captchaId}")]
        public IActionResult Get(string captchaId)
        {
            var captcha = _captchaManager.RefreshCaptcha(captchaId);
            return OkResult(captcha);
        }

        /// <summary>
        /// validate captcha
        /// </summary>      
        /// <param name="captcha"></param>
        /// <returns></returns>
        [HttpPost("ValidateCaptcha")]
        public IActionResult Check(CaptchaValidation captcha)
        {
            _captchaManager.ValidateCaptcha(captcha);
            return OkResult();
        }
    }
}
