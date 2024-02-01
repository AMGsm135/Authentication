using System;
using System.Collections.Generic;
using Amg.Authentication.Infrastructure.Enums;

namespace Amg.Authentication.Host.Dtos
{
    public class UserLoginResult
    {
        public Guid? UserId { get; set; }

        public SignInResultType Result { get; set; }

        public string AccessToken { get; set; }

        public UserLoginInfo UserInfo { get; set; }
    }


    public class UserLoginInfo
    {
        public Guid TokenId { get; set; }
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public List<RoleType> Roles { get; set; }
        public PersonType PersonType { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime ExpireAt { get; set; }
        public DateTime refreshExpireAt { get; set; }
    }

}
