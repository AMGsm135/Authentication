using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Amg.Authentication.Shared;
using Amg.Authentication.Shared.Attributes;
using Amg.Authentication.Shared.Enums;
using Amg.Authentication.Shared.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using SampleWebApp.Dtos;

namespace SampleWebApp.Authentication
{
    public class PermissionAuthorizationFilter : IAsyncAuthorizationFilter
    {
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        private readonly HttpClient _client;

        public string ServiceName { get; } = Constants.MicroServiceName;


        public PermissionAuthorizationFilter(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient(Constants.AuthenticationHttpClient);
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

                // در صورتی که برای منبع درخواستی حداقل یک پرمیژن ثبت شده باشد و کاربر ادمین نباشد
                var permissions = GetAppliedPermissions(context.ActionDescriptor);
                if (permissions.Any() && !UserIsSuperAdmin(userInfo))
                {
                    // در صورتی که کاربر حداقل یکی از پرمیژن های منبع درخواست شده را داشته باشد
                    foreach (var permission in permissions)
                    {
                        if ((permission.Role == RoleType.Any || UserHasRole(permission.Role, userInfo)) &&
                            (permission.Permission == null || await UserHasPermission(permission.Permission, userInfo, context.HttpContext)))
                            return;
                    }
                    context.Result = new ForbidResult();

                }
                // در صورتی که برای منبع پرمیژنی ثبت نشده باشد یا کاربر ادمین باشد، فقط توکن کاربر بررسی می شود
                else
                {
                    var request = new HttpRequestMessage(HttpMethod.Get, "check/token");
                    request.Headers.TryAddWithoutValidation("Cookie", context.HttpContext.Request.Headers["Cookie"].ToString());
                    request.Headers.TryAddWithoutValidation("X-Forwarded-For", GetRoutedClientIp(context.HttpContext));
                    var response = await _client.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    var result = JsonSerializer.Deserialize<ResponseMessage<bool>>(await response.Content.ReadAsStringAsync(), JsonOptions);
                    if (!result.Content)
                    {
                        context.Result = new UnauthorizedResult();
                    }
                }
            }
            catch
            {
                context.Result = new UnauthorizedResult();
            }
        }


        private async Task<bool> UserHasPermission(string permission, UserInfo userInfo, HttpContext context)
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"UserGroups/users/{userInfo.UserId}/isAuthorize?" +
                $"permissionName={Constants.MicroServiceName}-{permission}&serviceName={Constants.MicroServiceName}");
            request.Headers.TryAddWithoutValidation("Cookie", context.Request.Headers["Cookie"].ToString());
            request.Headers.TryAddWithoutValidation("X-Forwarded-For", GetRoutedClientIp(context));
            var response = await _client.SendAsync(request);
            if (!response.IsSuccessStatusCode)
                return false;

            var result = JsonSerializer.Deserialize<ResponseMessage<bool>>(await response.Content.ReadAsStringAsync(), JsonOptions);
            return result.Content;
        }

        private static bool UserHasRole(RoleType role, UserInfo userInfo)
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
            return userInfo.Roles.Contains("SuperAdmin");
        }


        private List<(RoleType Role, string Permission)> GetAppliedPermissions(ActionDescriptor descriptor)
        {
            if (!(descriptor is ControllerActionDescriptor actionDescriptor))
                return new List<(RoleType Role, string Permission)>();

            var permissions = actionDescriptor.MethodInfo.GetCustomAttributes<CheckPermissionAttribute>()
                .Concat(actionDescriptor.ControllerTypeInfo.GetCustomAttributes<CheckPermissionAttribute>()).ToList();

            return permissions.Select(i => (i.Role, i.Permission?.ToString())).ToList();
        }


        private string GetRoutedClientIp(HttpContext context)
        {
            // a.ammari:
            // در صورتی که سرویس پشت یک ریورس پروکسی باشد سعی می کنیم اطلاعات آی پی مبدا را استخراج کنیم
            // ریورس پروکسی باید اطلاعات آی پی مبدا را توسط یکی از هدر های استاندارد ارسال کند.

            var forwardedFor = context.Request.Headers["X-Forwarded-For"];
            var forwarded = context.Request.Headers["Forwarded"];

            if (forwardedFor != StringValues.Empty && !string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.ToString();
            }

            if (forwarded != StringValues.Empty && !string.IsNullOrEmpty(forwarded))
            {
                return forwarded.ToString()
                    .Split(';', StringSplitOptions.RemoveEmptyEntries)
                    .FirstOrDefault(i => i.Trim()
                        .StartsWith("for=", StringComparison.OrdinalIgnoreCase))?
                    .Split('=', StringSplitOptions.RemoveEmptyEntries)
                    .Last().Trim();
            }

            return context.Connection.RemoteIpAddress.ToString();
        }

    }
}
