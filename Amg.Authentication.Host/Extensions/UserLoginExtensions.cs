using System.Linq;
using Amg.Authentication.Application.Contract.Dtos;
using Amg.Authentication.Host.Dtos;
using Amg.Authentication.Infrastructure.Shared;

namespace Amg.Authentication.Host.Extensions
{
    public static class UserLoginExtensions
    {
        public static UserLoginResult ToDto(this SignInResult signInResult)
        {
            if (signInResult == null)
                return null;

            return new UserLoginResult()
            {
                UserId = signInResult.UserId,
                Result = signInResult.Result,
                AccessToken = signInResult.AccessToken,
                UserInfo = signInResult.Ticket?.ToDto()
            };
        }

        public static UserLoginInfo ToDto(this UserTicket ticket)
        {
            if (ticket == null)
                return null;

            return new UserLoginInfo()
            {
                TokenId = ticket.TokenId,
                UserId = ticket.UserId,
                UserName = ticket.UserName,
                Name = ticket.Name,
                PersonType = ticket.PersonType,
                Roles = ticket.Roles.Select(i => i).ToList(), // copy
                ExpireAt = ticket.TokenExpireAt,
                refreshExpireAt = ticket.RefreshExpireAt
            };
        }
    }
}
