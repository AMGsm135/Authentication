using Amg.Authentication.DomainModel.Modules.Permissions;
using Amg.Authentication.QueryModel.Dtos.Authorization;

namespace Amg.Authentication.QueryService.Extensions
{
    public static class PermissionExtensions
    {
        public static PermissionDto ToDto(this Permission permission)
        {
            if (permission == null)
                return null;

            return new PermissionDto()
            {
                Id = permission.Id,
                Category = permission.Category,
                Description = permission.Category,
                Name = permission.Name,
                ServiceName = permission.ServiceName,
            };
        }
    }
}
