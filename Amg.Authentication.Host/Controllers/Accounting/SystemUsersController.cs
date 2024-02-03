using System;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Command.Accounting.FundUsers;
using Amg.Authentication.Host.SeedWorks;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.QueryModel.Services.Accounting;
using Gridify;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amg.Authentication.Host.Controllers.Accounting
{
    [AllowAnonymous]
    [Route(Constants.ApiPrefix + "/v1/[controller]")]
    public class SystemUsersController : ApiControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly ICommandValidator _commandValidator;
        private readonly IAccountQueryService _accountQueryService;

        public SystemUsersController(ICommandBus commandBus, ICommandValidator commandValidator,
            IAccountQueryService accountQueryService)
        {
            _commandBus = commandBus;
            _commandValidator = commandValidator;
            _accountQueryService = accountQueryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetByFilter([FromQuery] GridifyQuery query)
        {
            var users = await _accountQueryService.GetFundUsersByFilterAsync(query);
            return OkResult(users);
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetById(Guid userId)
        {
            var user = await _accountQueryService.GetUserById(userId);
            return OkResult(user);
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(RegisterSystemUserCommand command)
        {
            command.Id = Guid.NewGuid();
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult(command.Id);
        }

        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(Guid userId, UpdateSystemUserCommand command)
        {
            command.UserId = userId;
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult();
        }

        [HttpGet("{userId}/roles")]
        public async Task<IActionResult> GetRoles(Guid userId)
        {
            var userRoles = await _accountQueryService.GetUserRoles(userId);
            return OkResult(userRoles);
        }

        [HttpPost("{userId}/roles")]
        public async Task<IActionResult> AssignRole(Guid userId, AssignRoleToFundUserCommand command)
        {
            command.UserId = userId;
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult();

        }

        [HttpDelete("{userId}/roles")]
        public async Task<IActionResult> UnAssignRole(Guid userId, UnAssignRoleFromFundUserCommand command)
        {
            command.UserId = userId;
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult();

        }


        [HttpPut("{userId}/activate")]
        public async Task<IActionResult> Activate(Guid userId)
        {
            var command = new ActivateUserCommand()
            {
                UserId = userId
            };
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult();
        }

        [HttpPut("{userId}/deactivate")]
        public async Task<IActionResult> Deactivate(Guid userId)
        {
            var command = new DeactivateUserCommand()
            {
                UserId = userId
            };
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult();
        }

    }
}
