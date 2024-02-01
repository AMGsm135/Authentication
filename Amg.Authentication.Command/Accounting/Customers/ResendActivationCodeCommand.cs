using System;
using Amg.Authentication.Infrastructure.Base;

namespace Amg.Authentication.Command.Accounting.Customers
{
    public class ResendActivationCodeCommand : ValidateCaptchaCommand
    {
        public Guid UserId { get; set; }
    }
}
