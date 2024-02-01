using Amg.Authentication.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Amg.Authentication.Persistence.Contexts
{
    public class DatabaseContext : DbContext
    {

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            modelBuilder.UseEnumToStringConverter();
        }
    }
}
