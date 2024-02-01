using System;
using Amg.Authentication.Infrastructure.Base;

namespace Amg.Authentication.Command.Accounting.TwoFactors
{
    public class DisableOtpTwoFactorCommand : CommandBase
    {
        public Guid UserId { get; set; }
    }
}