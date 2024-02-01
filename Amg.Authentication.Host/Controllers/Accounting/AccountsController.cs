using System.Threading.Tasks;
using Amg.Authentication.Application.Contract;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Host.SeedWorks;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.QueryModel.Services.Accounting;
using Microsoft.AspNetCore.Mvc;

namespace Amg.Authentication.Host.Controllers.Accounting
{
    [Route(Constants.ApiPrefix + "/v1/[controller]")]
    public class AccountsController : ApiControllerBase
    {
        private readonly ICommandBus _commandBus;
        private readonly ICommandValidator _commandValidator;
        private readonly IAccountQueryService _accountQueryService;

        public AccountsController(ICommandBus commandBus, ICommandValidator commandValidator,
            IAccountQueryService accountQueryService)
        {
            _commandBus = commandBus;
            _commandValidator = commandValidator;
            _accountQueryService = accountQueryService;
        }


        [HttpGet("{userName}/exists")]
        public async Task<IActionResult> IsUserExists(string userName)
        {
            var isUserExists = await _accountQueryService.IsUserExists(userName);
            return isUserExists ? (IActionResult)Ok() : NotFound();
        }


    }

}
