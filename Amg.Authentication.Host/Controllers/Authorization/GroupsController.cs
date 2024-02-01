using System;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Command.Authorization.Groups;
using Amg.Authentication.Host.SeedWorks;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.QueryModel.Services.Authorization;
using Gridify;
using Microsoft.AspNetCore.Mvc;

namespace Amg.Authentication.Host.Controllers.Authorization
{
    [Route(Constants.ApiPrefix + "/v1/[controller]")]
    public class GroupsController : ApiControllerBase
    {
        private readonly IGroupQueryService _groupQueryService;
        private readonly ICommandBus _commandBus;
        private readonly ICommandValidator _commandValidator;

        public GroupsController(ICommandBus commandBus, ICommandValidator commandValidator,
            IGroupQueryService groupQueryService)
        {
            _groupQueryService = groupQueryService;
            _commandBus = commandBus;
            _commandValidator = commandValidator;
        }

        /// <summary>
        /// دریافت لیست گروه ها
        /// </summary>
        /// <returns></returns>
        [HttpGet("")]
        public async Task<IActionResult> GetByFilter([FromQuery] GridifyQuery query)
        {
            var groups = await _groupQueryService.GetByFilterAsync(query);
            return OkResult(groups);
        }

        /// <summary>
        /// دریافت اطلاعات گروه
        /// </summary>
        /// <param name="groupId">شناسه گروه</param>
        /// <returns></returns>
        [HttpGet("{groupId}")]
        public async Task<IActionResult> GetById(Guid groupId)
        {
            var group = await _groupQueryService.GetByIdAsync(groupId);
            return OkResult(group);
        }

        /// <summary>
        /// ایجاد گروه
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("")]
        public async Task<IActionResult> Post(AddGroupCommand command)
        {
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult();
        }

        /// <summary>
        /// ویرایش اطلاعات گروه
        /// </summary>
        /// <param name="groupId">شناسه گروه</param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut("{groupId}")]
        public async Task<IActionResult> Put(Guid groupId, UpdateGroupCommand command)
        {
            command.Id = groupId;
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult();
        }

        /// <summary>
        /// دریافت لیست دسترسی های گروه
        /// </summary>
        /// <param name="groupId">شناسه گروه</param>
        /// <returns></returns>
        [HttpGet("{groupId}/permissions")]
        public async Task<IActionResult> GetPermissions(Guid groupId)
        {
            var groupPermissions = await _groupQueryService.GetGroupPermissionsAsync(groupId);
            return OkResult(groupPermissions);
        }

        /// <summary>
        /// دریافت عناوین سطوح دسترسی های گروه خواسته شده
        /// </summary>
        /// <param name="groupId">شناسه گروه</param>
        /// <returns></returns>
        [HttpGet("{groupId}/permissions/names")]
        public async Task<IActionResult> GetAllNames(Guid groupId)
        {
            var permissionsNames = await _groupQueryService.GetGroupPermissionNamesAsync(groupId);
            return OkResult(permissionsNames);
        }

        /// <summary>
        /// دریافت عناوین سطوح دسترسی های گروه های خواسته شده
        /// </summary>
        /// <param name="groupIds">شناسه های گروه ها</param>
        /// <returns></returns>
        [HttpGet("permissions/names")]
        public async Task<IActionResult> GetAllNames([FromQuery] Guid[] groupIds)
        {
            var permissionsNames = await _groupQueryService.GetGroupPermissionNamesAsync(groupIds);
            return OkResult(permissionsNames);
        }

        /// <summary>
        /// انتساب دسترسی به گروه
        /// </summary>
        /// <param name="groupId">شناسه دسترسی</param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("{groupId}/permissions")]
        public async Task<IActionResult> AssignPermission(Guid groupId, AssignPermissionToGroupCommand command)
        {
            command.GroupId = groupId;
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult();
        }

        /// <summary>
        /// آیا گروه به سطح دسترسی مورد نظر دسترسی دارد ؟
        /// </summary>
        /// <param name="groupId">شناسه گروه</param>
        /// <param name="permissionName">عنوان دسترسی</param>
        /// <param name="serviceName">نام سرویس</param>
        /// <returns></returns>
        [HttpGet("{groupId}/permissions/{permissionName}/isAuthorize")]
        public async Task<IActionResult> IsAuthorize(Guid groupId, string permissionName, string serviceName)
        {
            var isAuthorize = await _groupQueryService.HasGroupPermissionAsync(permissionName, serviceName, groupId);
            return OkResult(isAuthorize);
        }

        /// <summary>
        /// آیا گروه های داده شده به سطح دسترسی مورد نظر دسترسی دارد ؟
        /// </summary>
        /// <param name="permissionName">عنوان دسترسی</param>
        /// <param name="groupIds">شناسه گروه ها</param>
        /// <param name="serviceName">نام سرویس</param>
        /// <returns></returns>
        [HttpGet("permissions/{permissionName}/isAuthorize")]
        public async Task<IActionResult> IsAuthorize(string permissionName, string serviceName, [FromQuery] Guid[] groupIds)
        {
            var isAuthorize = await _groupQueryService.HasGroupPermissionAsync(permissionName, serviceName, groupIds);
            return OkResult(isAuthorize);
        }
    }
}
