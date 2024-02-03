using System;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Command.Accounting.Customers;
using Amg.Authentication.Host.SeedWorks;
using Amg.Authentication.Infrastructure.Base;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amg.Authentication.Host.Controllers.Accounting
{
    [Route(Constants.ApiPrefix + "/v1/[controller]")]
    public class CustomersController : ApiControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly ICommandValidator _commandValidator;

        public CustomersController(ICommandBus commandBus, ICommandValidator commandValidator)
        {
            _commandBus = commandBus;
            _commandValidator = commandValidator;
        }


        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCustomers(RegisterCustomerCommand command)
        {
            command.Id = Guid.NewGuid();
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult(command.Id);
        }

        [HttpPut("{userId}/update-phoneNumber")]
        public async Task<IActionResult> UpdatePhoneNumber(Guid userId, UpdateCustomerPhoneNumberCommand command)
        {
            command.UserId = userId;
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult("شماره رابط با موفقیت تغییر یافت");
        }

        [HttpPut("{userId}/update-info")]
        public async Task<IActionResult> UpdateInfo(Guid userId, UpdateCustomerCommand command)
        {
            command.AccessToken = AccessToken;
            command.UserId = userId;
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult("اطلاعات با موفقیت تغییر یافت");
        }

        [HttpPost("{userId}/activation/resend")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendActivationCode(Guid userId, ResendActivationCodeCommand command)
        {
            command.UserId = userId;
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult();
        }

        [HttpPost("{userId}/activation/verify")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyActivationCode(Guid userId, VerifyActivationCodeCommand command)
        {
            command.UserId = userId;
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult();
        }



    }
}
