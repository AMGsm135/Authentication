using System;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.Infrastructure.Enums;

namespace Amg.Authentication.Command.Accounting.FundUsers
{
    public class UnAssignRoleFromFundUserCommand : CommandBase
    {
        public Guid UserId { get; set; }

        public RoleType Role { get; set; }

        public override void Validate()
        {
            base.Validate();
        }
    }
}