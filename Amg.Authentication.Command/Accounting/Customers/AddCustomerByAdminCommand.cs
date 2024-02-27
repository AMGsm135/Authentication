using Amg.Authentication.Infrastructure.Base;
using System;


namespace Amg.Authentication.Command.Accounting.Customers
{
    public class AddCustomerByAdminCommand : CommandBase
    {

        public Guid Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string City { get; set; }

        public string Province { get; set; }

        public string PhoneNumber { get; set; }
    }
}
