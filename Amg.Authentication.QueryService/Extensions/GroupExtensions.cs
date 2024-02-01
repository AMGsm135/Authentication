using Amg.Authentication.DomainModel.Modules.Groups;
using Amg.Authentication.QueryModel.Dtos.Authorization;

namespace Amg.Authentication.QueryService.Extensions
{
    public static class GroupExtensions
    {
        public static GroupDto ToDto(this Group group, int totalUsers)
        {
            if (group == null)
                return null;

            return new GroupDto()
            {
                Id = group.Id,
                CreationDate = group.CreationDate,
                Name = group.Name,
                TotalUsers = totalUsers
            };
        }
    }
}
