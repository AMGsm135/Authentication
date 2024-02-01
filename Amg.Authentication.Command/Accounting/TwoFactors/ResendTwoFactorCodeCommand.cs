using System;
using Amg.Authentication.Infrastructure.Base;

namespace Amg.Authentication.Command.Accounting.TwoFactors
{
    public class ResendTwoFactorCodeCommand : ValidateCaptchaCommand
    {
        public Guid UserId { get; set; }
    }
}