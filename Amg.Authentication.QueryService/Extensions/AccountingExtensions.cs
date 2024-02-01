using System.Collections.Generic;
using Amg.Authentication.DomainModel.Modules.Users;
using Amg.Authentication.QueryModel.Dtos.Accounting;

namespace Amg.Authentication.QueryService.Extensions
{
    public static class AccountingExtensions
    {
        public static UserDto ToDto(this User user, IList<string> userRoles)
        {
            if (user == null)
                return null;

            return new UserDto()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.Name.Contains('|') ? user.Name.Split('|')[0] : user.Name,
                LastName = user.Name.Contains('|') ? user.Name.Split('|')[1] : user.Name,
                IsActive = user.IsActive,
                PersonType = user.PersonType,
                PhoneNumber = user.PhoneNumber,
                RegisterDateTime = user.RegisterDateTime,
                Roles = userRoles,
                TwoFactorEnabled = user.TwoFactorEnabled,
                LockoutEnabled = user.LockoutEnabled
            };
        }
    }
}
