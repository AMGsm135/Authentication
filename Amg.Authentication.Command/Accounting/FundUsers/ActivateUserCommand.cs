using System;
using Amg.Authentication.Infrastructure.Base;

namespace Amg.Authentication.Command.Accounting.FundUsers
{
    public class ActivateUserCommand : CommandBase
    {
        public Guid UserId { get; set; }
    }
}
