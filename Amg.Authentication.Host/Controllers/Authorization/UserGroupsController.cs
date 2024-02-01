using System;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Command.Authorization.Groups;
using Amg.Authentication.Host.SeedWorks;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.QueryModel.Services.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Amg.Authentication.Host.Controllers.Authorization
{
    [Route(Constants.ApiPrefix + "/v1/[controller]")]
    public class UserGroupsController : ApiControllerBase
    {
        private readonly IGroupQueryService _groupQueryService;
        private readonly ICommandBus _commandBus;
        private readonly ICommandValidator _commandValidator;
        private readonly ISignInService _signInService;


        public UserGroupsController(ICommandBus commandBus, ICommandValidator commandValidator,
            IGroupQueryService groupQueryService, ISignInService signInService)
        {
            _groupQueryService = groupQueryService;
            _signInService = signInService;
            _commandBus = commandBus;
            _commandValidator = commandValidator;
        }

        /// <summary>
        /// دریافت لیست گروه های کاربر
        /// </summary>
        /// <param name="userId">شناسه کاربر</param>
        /// <returns></returns>
        [HttpGet("users/{userId}/groups")]
        public async Task<IActionResult> GetUserGroups(Guid userId)
        {
            var userGroups = await _groupQueryService.GetUserGroupsAsync(userId);
            return OkResult(userGroups);
        }


        /// <summary>
        /// دریافت نام گروه های کاربری
        /// </summary>
        /// <param name="userId">شناسه کاربر</param>
        /// <returns></returns>
        [HttpGet("users/{userId}/groups/names")]
        public async Task<IActionResult> GetUserGroupsNames(Guid userId)
        {
            var userGroupsNames = await _groupQueryService.GetUserGroupNamesAsync(userId);
            return OkResult(userGroupsNames);
        }

        /// <summary>
        /// دریافت لیست نام های دسترسی کاربر 
        /// </summary>
        /// <param name="userId">شناسه کاربرس</param>
        /// <param name="serviceName">نام سرویس</param>
        /// <returns></returns>
        [HttpGet("users/{userId}/permissions/names")]
        public async Task<IActionResult> GetUserPermissionsNames(Guid userId, string serviceName)
        {
            var permissions = await _groupQueryService.GetUserPermissionNamesAsync(userId, serviceName);
            return OkResult(permissions);
        }

        /// <summary>
        /// انتصاب گروه به کاربران
        /// </summary>
        /// <param name="userId">شناسه کاربر</param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost("users/{userId}/groups")]
        public async Task<IActionResult> AssignGroupsToUser(Guid userId, AssignUserToGroupsCommand command)
        {
            command.UserId = userId;
            _commandValidator.Validate(command);
            await _commandBus.SendAsync(command);
            return OkResult();
        }

        /// <summary>
        /// دریافت اطلاعات Authorization کاربر
        /// </summary>
        /// <param name="userId">شناسه کاربر</param>
        /// <returns></returns>
        [HttpGet("users/{userId}/authorization-info")]
        public async Task<IActionResult> GetAuthorizationInfo(Guid userId)
        {
            var userGroupPermissions = await _groupQueryService.GetUserGroupPermissionsAsync(userId);
            return OkResult(userGroupPermissions);
        }

        /// <summary>
        /// آیا کاربر دسترسی مشخص شده را دارد ؟
        /// </summary>
        /// <param name="userId">شناسه کاربر</param>
        /// <param name="permissionName">نام دسترسی</param>
        /// <param name="serviceName">نام سرویس</param>
        /// <returns></returns>
        [HttpGet("users/{userId}/isAuthorize")]
        public async Task<IActionResult> IsAuthorize(Guid userId, string permissionName, string serviceName)
        {
            if (UserInfo == null || !await _signInService.IsTokenValid(UserInfo.TokenId, UserInfo.UserId))
                return Unauthorized();

            var result = await _groupQueryService.HasUserPermissionAsync(permissionName, serviceName, userId);
            return OkResult(result);
        }
    }
}
