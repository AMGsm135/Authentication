using System;
using System.Collections.Generic;
using Amg.Authentication.Infrastructure.Enums;
using Amg.Authentication.Infrastructure.Helpers;

namespace Amg.Authentication.Infrastructure.Shared
{
    public class UserTicket
    {
        public Guid TokenId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public List<RoleType> Roles { get; set; }
        public PersonType PersonType { get; set; }
        public DateTime TokenExpireAt { get; set; }
        public DateTime RefreshExpireAt { get; set; }

        public string RolesString
        {
            get => Roles != null ? string.Join(',', Roles) : string.Empty;
            set => Roles = RolesParser.ToRoleTypes((value ?? "").Split(','));
        }

    }
}
