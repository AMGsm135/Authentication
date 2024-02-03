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
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                PersonType = user.PersonType,
                PhoneNumber = user.PhoneNumber,
                Province = user.Province,
                City = user.City,
                RegisterDateTime = user.RegisterDateTime,
                Roles = userRoles,
                TwoFactorEnabled = user.TwoFactorEnabled,
                LockoutEnabled = user.LockoutEnabled
            };
        }
    }
}
