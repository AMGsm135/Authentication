using System.Collections.Generic;
using System.Threading.Tasks;
using Amg.Authentication.Infrastructure.Base;
using Amg.Authentication.QueryModel.Dtos.Accounting;

namespace Amg.Authentication.QueryModel.Services.Accounting
{
    public interface IRoleQueryService : IQueryService
    {
        /// <summary>
        /// دریافت لیست نقش ها به همراه تعداد کاربران منتسب به آن
        /// </summary>
        /// <returns></returns>
        Task<List<RoleInfoDto>> GetRoleInfos();


        /// <summary>
        /// دریافت یک نقش به همراه تعداد کاربران منتسب به آن
        /// </summary>
        /// <returns></returns>
        Task<RoleInfoDto> GetRoleInfo(string roleName);
    }
}
