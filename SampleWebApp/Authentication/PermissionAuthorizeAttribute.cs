using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

namespace SampleWebApp.Authentication
{
    public class PermissionAuthorizeAttribute : Attribute, IFilterFactory
    {
        public bool IsReusable { get; } = false;

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var filter = serviceProvider.GetRequiredService<PermissionAuthorizationFilter>();
            return filter;
        }


    }
}
