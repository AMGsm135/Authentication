using System.Threading.Tasks;
using Amg.Authentication.Application.Contract;
using Amg.Authentication.Host.SeedWorks;
using Amg.Authentication.QueryModel.Services.Accounting;
using Microsoft.AspNetCore.Mvc;

namespace Amg.Authentication.Host.Controllers.Accounting
{
    [Route(Constants.ApiPrefix + "/v1/[controller]")]
	public class RolesController : ApiControllerBase
    {
        private readonly IRoleQueryService _roleQueryService;

        public RolesController(IRoleQueryService roleQueryService)
        {
            _roleQueryService = roleQueryService;
        }

        [HttpGet("")]
		public async Task<IActionResult> GetAll()
        {
            var roles = await _roleQueryService.GetRoleInfos();
            return OkResult(roles);
		}

		[HttpGet("{roleName}")]
		public async Task<IActionResult> GetByName(string roleName)
		{
            var role = await _roleQueryService.GetRoleInfo(roleName);
            return OkResult(role);
		}

	}
}
