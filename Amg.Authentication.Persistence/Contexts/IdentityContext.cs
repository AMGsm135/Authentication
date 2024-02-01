using System;
using Amg.Authentication.DomainModel.Modules.Roles;
using Amg.Authentication.DomainModel.Modules.Users;
using Amg.Authentication.Persistence.Extensions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Amg.Authentication.Persistence.Contexts
{
    public class IdentityContext : IdentityDbContext<User, Role, Guid>
    {

        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseEnumToStringConverter();
        }
    }
}
