using System;
using Amg.Authentication.Infrastructure.Base;

namespace Amg.Authentication.Command.Accounting.TwoFactors
{
    public class EnableOtpTwoFactorCommand : CommandBase
    {
        public Guid UserId { get; set; }
    }
}