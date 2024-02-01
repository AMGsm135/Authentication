using System;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract;
using Amg.Authentication.Application.Contract.Dtos;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Command.Accounting.TwoFactors;
using Amg.Authentication.Host.SeedWorks;
using Amg.Authentication.Infrastructure.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amg.Authentication.Host.Controllers.Accounting
{
    [Route(Constants.ApiPrefix + "/v1/[controller]")]
    public class TwoFactorController : ApiControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly ICommandValidator _commandValidator;

        public TwoFactorController(ICommandBus commandBus, ICommandValidator commandValidator)
        {
            _commandBus = commandBus;
            _commandValidator = commandValidator;
        }


        [HttpPut("{userId}/enable")]
        public async Task<IActionResult> Enable(Guid userId)
        {
            var command = new EnableTwoFactorCommand { UserId = userId };
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult();
        }


        [HttpPut("{userId}/disable")]
        public async Task<IActionResult> Disable(Guid userId)
        {
            var command = new DisableTwoFactorCommand { UserId = userId };
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult();
        }


        [HttpPut("{userId}/enable-otp")]
        public async Task<IActionResult> EnableOtp(Guid userId)
        {
            var command = new EnableOtpTwoFactorCommand { UserId = userId };
            _commandValidator.Validate(command);
            var result = await _commandBus.SendAsync<EnableOtpTwoFactorCommand, OtpActivationResult>(command);
            return OkResult(result);
        }


        [HttpPut("{userId}/disable-otp")]
        public async Task<IActionResult> DisableOtp(Guid userId)
        {
            var command = new DisableOtpTwoFactorCommand { UserId = userId };
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult();
        }


        [HttpPost("{userId}/resend-code")]
        [AllowAnonymous]
        public async Task<IActionResult> TwoFactorVerificationResend(Guid userId)
        {
            var command = new ResendTwoFactorCodeCommand { UserId = userId };
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult();
        }
    }
}
