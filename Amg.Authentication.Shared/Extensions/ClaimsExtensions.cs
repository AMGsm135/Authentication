using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Amg.Authentication.Shared.Extensions
{
    public static class ClaimsExtensions
    {
        public static UserInfo ToUserInfo(this IList<Claim> claims)
        {
            if (claims == null || !claims.Any())
                return null;

            if (!Guid.TryParse(GetClaim(claims, AuthConstants.TokenIdClaimType), out var tokenId))
                return null;

            if (!Guid.TryParse(GetClaim(claims, AuthConstants.UserIdClaimType), out var userId))
                return null;

            long.TryParse(GetClaim(claims, AuthConstants.TokenExpireClaimType), out var tokenExpires);
            var tokenExpireAt = new DateTime(tokenExpires);
            long.TryParse(GetClaim(claims, AuthConstants.RefreshExpireClaimType), out var refreshExpires);
            var refreshExpireAt = new DateTime(refreshExpires);

            var userInfo = new UserInfo
            {
                UserId = userId,
                TokenId = tokenId,
                Name = GetClaim(claims, AuthConstants.GivenNameClaimType),
                Roles = (GetClaim(claims, AuthConstants.UserRolesClaimType) ?? "")
                    .Split(new []{','}, StringSplitOptions.RemoveEmptyEntries)
                    .ToList(),
                PersonType = GetClaim(claims, AuthConstants.PersonTypeClaimType),
                UserName = GetClaim(claims, AuthConstants.UserNameClaimType),
                TokenExpireAt = tokenExpireAt,
                RefreshExpireAt = refreshExpireAt,
            };

            return userInfo;
        }

        private static string GetClaim(IList<Claim> claims, string type)
        {
            return claims.FirstOrDefault(i => i.Type == type)?.Value;
        }
    }
}
