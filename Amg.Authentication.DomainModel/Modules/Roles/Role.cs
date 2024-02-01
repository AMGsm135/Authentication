using System;
using Amg.Authentication.DomainModel.SeedWorks.Base;
using Microsoft.AspNetCore.Identity;

namespace Amg.Authentication.DomainModel.Modules.Roles
{
    public class Role : IdentityRole<Guid>, IAggregateRoot
    {
        /// <inheritdoc />
        public Role(string roleName) : base(roleName)
        {
            this.CreationDate = DateTime.Now;
        }

        public DateTime CreationDate { get; set; }

        // FOR ORM !
        private Role() { }

        public object GetId()
        {
            return this.Id;
        }
    }
}
