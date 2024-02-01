using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amg.Authentication.DomainModel.Modules.Permissions;
using Amg.Authentication.DomainModel.Modules.Permissions.Interfaces;
using Amg.Authentication.Persistence.Contexts;
using Amg.Authentication.Persistence.Repositories.Base;
using Gridify;
using Gridify.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Amg.Authentication.Persistence.Repositories
{
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        public PermissionRepository(DatabaseContext context) : base(context)
        {
        }

        /// <inheritdoc />
        public async Task<List<Permission>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Paging<Permission>> GetByFilterAsync(GridifyQuery query)
        {
            return await DbSet.GridifyAsync(query);
        }

        /// <inheritdoc />
        public async Task<List<Permission>> GetByIdsAsync(params Guid[] ids)
        {
            return await DbSet.Where(i => ids.Contains(i.Id)).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<Permission> GetByIdAsync(Guid id)
        {
            return await DbSet.FirstOrDefaultAsync(i => i.Id == id);
        }
    }
}
