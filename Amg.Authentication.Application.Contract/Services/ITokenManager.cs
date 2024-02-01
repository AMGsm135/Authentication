using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract.Dtos;
using Amg.Authentication.Infrastructure.Base;

namespace Amg.Authentication.Application.Contract.Services
{
    public interface ITokenManager : IApplicationService
    {
        Task AddToken(UserTokenItem item, TimeSpan expire);
        Task<UserTokenItem> GetToken(Guid tokenId, Guid userId);
        Task<UserTokenItem> GetTokenById(Guid tokenId);
        Task<List<UserTokenItem>> GetTokensByUserId(Guid userId);
        Task RemoveToken(Guid tokenId, Guid userId);
        Task RemoveToken(Guid tokenId);
    }
}