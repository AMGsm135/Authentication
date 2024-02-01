using System;
using System.Linq;
using System.Threading.Tasks;
using Amg.Authentication.DomainModel.Modules.Permissions.Interfaces;
using Amg.Authentication.QueryModel.Dtos.Authorization;
using Amg.Authentication.QueryModel.Services.Authorization;
using Amg.Authentication.QueryService.Extensions;
using Gridify;

namespace Amg.Authentication.QueryService.Modules.Authorization
{
    public class PermissionQueryService : IPermissionQueryService
    {
        private readonly IPermissionRepository _permissionRepository;

        public PermissionQueryService(IPermissionRepository permissionRepository)
        {
            _permissionRepository = permissionRepository;
        }

        public async Task<Paging<PermissionDto>> GetByFilterAsync(GridifyQuery query)
        {
            var permissions = await _permissionRepository.GetByFilterAsync(query);
            return new Paging<PermissionDto>(permissions.Count, permissions.Data.Select(i => i.ToDto()));
        }

        public async Task<PermissionDto> GetByIdAsync(Guid id)
        {
            var permission = await _permissionRepository.GetByIdAsync(id);
            return permission.ToDto();
        }
    }
}
