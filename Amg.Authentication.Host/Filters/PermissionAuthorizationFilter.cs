using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Infrastructure.Enums;
using Amg.Authentication.QueryModel.Services.Authorization;
using Amg.Authentication.Shared;
using Amg.Authentication.Shared.Attributes;
using Amg.Authentication.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Amg.Authentication.Host.Filters
{
    public class PermissionAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly IGroupQueryService _groupQueryService;
        private readonly ISignInService _signInService;

        public string ServiceName { get; } = Constants.MicroServiceName;

        public PermissionAuthorizationFilter(IGroupQueryService groupQueryService, ISignInService signInService)
        {
            _groupQueryService = groupQueryService;
            _signInService = signInService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                // در صورتی که منبع درخواست شده نیاز به تایید هویت نداشته باشد
                if (ResourceIsAnonymous(context.ActionDescriptor))
                    return;

                // در صورتی که مشخصات کاربر قابل استخراج نباشد، کاربر لاگین نباشد یا توکن وی منقضی شده باشد
                var userInfo = context.HttpContext.User?.Claims?.ToList().ToUserInfo();
                if (userInfo?.Roles == null || !userInfo.Roles.Any())
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                // بررسی توکن کاربر
                if (!await _signInService.IsTokenValid(userInfo.TokenId, userInfo.UserId))
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                // در صورتی که برای منبع درخواستی حداقل یک پرمیژن ثبت شده باشد و کاربر ادمین نباشد
                var permissions = GetAppliedPermissions(context.ActionDescriptor);
                if (permissions.Any() && !UserIsSuperAdmin(userInfo))
                {
                    // در صورتی که کاربر حداقل یکی از پرمیژن های منبع درخواست شده را داشته باشد
                    foreach (var permission in permissions)
                    {
                        if ((permission.Role == Shared.Enums.RoleType.Any || UserHasRole(permission.Role, userInfo)) &&
                            (permission.Permission == null || await UserHasPermission(permission.Permission, userInfo)))
                            return;
                    }

                    // در صورتی که کاربر پرمیژن مد نظر را نداشته باشد.
                    context.Result = new ForbidResult();
                }
                else
                {
                    // do more checks if needed
                    return;
                }
            }
            catch
            {
                context.Result = new UnauthorizedResult();
            }
        }

        private async Task<bool> UserHasPermission(string permission, UserInfo userInfo)
        {
            return await _groupQueryService.HasUserPermissionAsync($"{ServiceName}-{permission}", ServiceName, userInfo.UserId);
        }

        private static bool UserHasRole(Shared.Enums.RoleType role, UserInfo userInfo)
        {
            return userInfo.Roles.Contains(role.ToString());
        }

        private static bool ResourceIsAnonymous(ActionDescriptor descriptor)
        {
            if (!(descriptor is ControllerActionDescriptor actionDescriptor))
                return false; // worse case
            var attributes = actionDescriptor.MethodInfo.GetCustomAttributes<AllowAnonymousAttribute>();
            if (!attributes.Any())
            {
                return actionDescriptor.ControllerTypeInfo.GetCustomAttributes<AllowAnonymousAttribute>().Any() &&
                       !actionDescriptor.MethodInfo.GetCustomAttributes<AuthorizeAttribute>().Any();
            }

            return true;
        }

        private static bool UserIsSuperAdmin(UserInfo userInfo)
        {
            if (userInfo?.Roles == null || !userInfo.Roles.Any())
                return false;
            return userInfo.Roles.Contains(RoleType.SuperAdmin.ToString());
        }

        private List<(Shared.Enums.RoleType Role, string Permission)> GetAppliedPermissions(ActionDescriptor descriptor)
        {
            if (!(descriptor is ControllerActionDescriptor actionDescriptor))
                return new List<(Shared.Enums.RoleType Role, string Permission)>();

            var permissions = actionDescriptor.MethodInfo.GetCustomAttributes<CheckPermissionAttribute>()
                .Concat(actionDescriptor.ControllerTypeInfo.GetCustomAttributes<CheckPermissionAttribute>()).ToList();

            return permissions.Select(i => (i.Role, i.Permission?.ToString())).ToList();
        }


    }
}
