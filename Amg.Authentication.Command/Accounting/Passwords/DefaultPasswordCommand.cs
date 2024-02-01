using System;
using Amg.Authentication.Infrastructure.Base;

namespace Amg.Authentication.Command.Accounting.Passwords
{
    public class DefaultPasswordCommand : CommandBase
    {
        public Guid UserId { get; set; }
    }
}
