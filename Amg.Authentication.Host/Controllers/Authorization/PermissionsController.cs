using System;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract;
using Amg.Authentication.Host.SeedWorks;
using Amg.Authentication.QueryModel.Services.Authorization;
using Gridify;
using Microsoft.AspNetCore.Mvc;

namespace Amg.Authentication.Host.Controllers.Authorization
{
    [Route(Constants.ApiPrefix + "/v1/[controller]")]
	public class PermissionsController : ApiControllerBase
	{
		private readonly IPermissionQueryService _permissionQueryService;

		public PermissionsController(IPermissionQueryService permissionQueryService)
        {
            _permissionQueryService = permissionQueryService;
        }

		/// <summary>
		/// دریافت لیست دسترسی ها
		/// </summary>
		/// <param name="query">اطلاعات پیجینگ</param>
		/// <returns></returns>
		[HttpGet("")]
		public async Task<IActionResult> GetAll([FromQuery] GridifyQuery query)
		{
			var permissions = await _permissionQueryService.GetByFilterAsync(query);
			return OkResult(permissions);
		}

		/// <summary>
		/// دریافت اطلاعات دسترسی با استفاده از شناسه
		/// </summary>
		/// <param name="id">شناسه دسترسی</param>
		/// <returns></returns>
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var permission = await _permissionQueryService.GetByIdAsync(id);
            return OkResult(permission);
        }

	}
}
