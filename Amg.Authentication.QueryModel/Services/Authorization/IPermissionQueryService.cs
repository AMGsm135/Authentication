using System;
using System.Threading.Tasks;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.QueryModel.Dtos.Authorization;
using Gridify;

namespace Amg.Authentication.QueryModel.Services.Authorization
{
    public interface IPermissionQueryService : IQueryService
    {
        Task<Paging<PermissionDto>> GetByFilterAsync(GridifyQuery query);
        Task<PermissionDto> GetByIdAsync(Guid id);

    }
}
