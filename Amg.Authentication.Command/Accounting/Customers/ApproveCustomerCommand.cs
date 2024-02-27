using Amg.Authentication.Infrastructure.Base;
using System;

namespace Amg.Authentication.Command.Accounting.Customers
{
    public class ApproveCustomerCommand : CommandBase
    {
        public Guid Id { get; set; }

        public bool IsApproved { get; set; }
    }
}
