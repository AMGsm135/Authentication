using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Amg.Authentication.Persistence.Contexts
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DatabaseContext _context;

        public UnitOfWork(DatabaseContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            FixPersianCharacters();
            return _context.SaveChangesAsync(cancellationToken);
        }

        #region PrivateMethods

        private void FixPersianCharacters()
        {
            var changedEntities = _context.ChangeTracker.Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);
            foreach (var item in changedEntities)
            {
                if (item.Entity == null)
                    continue;

                var properties = item.Entity.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

                foreach (var property in properties)
                {
                    var val = (string)property.GetValue(item.Entity, null);

                    if (val.HasValue())
                    {
                        var newVal = val.Fa2En().FixPersianChars();
                        if (newVal == val)
                            continue;
                        property.SetValue(item.Entity, newVal, null);
                    }
                }
            }
        }

        #endregion
    }

}
