using System;
using System.Collections.Generic;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.Infrastructure.Enums;

namespace Amg.Authentication.Command.Accounting.FundUsers
{
    public class AssignRoleToFundUserCommand : CommandBase
    {
        public Guid UserId { get; set; }
        public List<RoleType> Roles { get; set; }

        public override void Validate()
        {
            base.Validate();
        }
    }
}
