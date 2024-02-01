using System;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Command.Accounting.Passwords;
using Amg.Authentication.Host.SeedWorks;
using Amg.Authentication.Infrastructure.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amg.Authentication.Host.Controllers.Accounting
{
    [Route(Constants.ApiPrefix + "/v1/[controller]")]
    public class PasswordController : ApiControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly ICommandValidator _commandValidator;

        public PasswordController(ICommandBus commandBus, ICommandValidator commandValidator)
        {
            _commandBus = commandBus;
            _commandValidator = commandValidator;
        }

        
        [HttpPost("forget")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgetPasswordCommand command)
        {
            _commandValidator.Validate(command);
            var userId = await _commandBus.SendAsync<ForgetPasswordCommand, Guid>(command);
            return OkResult(userId);
        }

        [HttpPost("reset/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(Guid userId, ResetPasswordCommand command)
        {
            command.UserId = userId;
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult("Password Reset Successfully");
        }

        [HttpPut("default/{userId}")]
        public async Task<IActionResult> DefaultPassword(Guid userId)
        {
            var command = new DefaultPasswordCommand { UserId = userId };
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult();
        }

        [HttpPut("restore/{userId}")]
        public async Task<IActionResult> RestorePassword(Guid userId)
        {
            var command = new RestorePasswordCommand { UserId = userId };
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult();
        }

        [HttpPut("change")]
        public async Task<IActionResult> ChangePassword(ChangePasswordCommand command)
        {
            command.UserId = UserInfo.UserId;
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult();
        }

        [HttpPost("reset/{userId}/by-admin")]
        public async Task<IActionResult> ResetPasswordByAdmin(Guid userId)
        {
            var command = new ResetPasswordByAdminCommand { UserId = userId };
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult();
        }

    }
}
