using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Infrastructure.Enums;
using Amg.Authentication.Infrastructure.Helpers;
using Amg.Authentication.Infrastructure.Settings;
using Amg.Authentication.Infrastructure.Shared;
using Amg.Authentication.Shared;
using Amg.Authentication.Shared.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Amg.Authentication.Application.Services
{
    public class JwtFactory : IJwtFactory
    {
        private readonly JwtTokenSettings _jwtTokenSettings;

        public JwtFactory(IOptions<JwtTokenSettings> jwtTokenSettings)
        {
            _jwtTokenSettings = jwtTokenSettings.Value;
        }


        public string GenerateToken(UserTicket userTicket)
        {
            var claims = new Dictionary<string, object>()
            {
                [AuthConstants.TokenIdClaimType] = userTicket.TokenId.ToString(),
                [AuthConstants.UserIdClaimType] = userTicket.UserId.ToString(),
                [AuthConstants.UserNameClaimType] = userTicket.UserName,
                [AuthConstants.UserRolesClaimType] = userTicket.RolesString,
                [AuthConstants.GivenNameClaimType] = userTicket.Name,
                [AuthConstants.PersonTypeClaimType] = userTicket.PersonType.ToString(),
                [AuthConstants.TokenExpireClaimType] = userTicket.TokenExpireAt.Ticks,
                [AuthConstants.RefreshExpireClaimType] = userTicket.RefreshExpireAt.Ticks,
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            //tokenHandler.
            var encryptionCredentials = new EncryptingCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtTokenSettings.DecryptionKey)),
                SecurityAlgorithms.Aes256KW,
                SecurityAlgorithms.Aes128CbcHmacSha256);
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtTokenSettings.ValidationKey)),
                SecurityAlgorithms.HmacSha256Signature);

            var token = tokenHandler.CreateToken(new SecurityTokenDescriptor()
            {
                Audience = _jwtTokenSettings.Audience,
                Claims = claims,
                Expires = userTicket.TokenExpireAt,
                NotBefore = DateTime.Now,
                IssuedAt = DateTime.Now,
                Issuer = _jwtTokenSettings.Issuer,
                //Subject = new GenericIdentity(userTicket.UserName),
                EncryptingCredentials = encryptionCredentials,
                SigningCredentials = signingCredentials,
            });

            var encodedJwt = tokenHandler.WriteToken(token);
            return encodedJwt;
        }


        public UserTicket DecodeToken(string token, bool validateLifetime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = _jwtTokenSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtTokenSettings.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtTokenSettings.ValidationKey)),
                TokenDecryptionKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtTokenSettings.DecryptionKey)),
                ValidateLifetime = validateLifetime,
                NameClaimType = AuthConstants.UserNameClaimType,
                RoleClaimType = AuthConstants.UserRolesClaimType
            }, out var jwtToken);
            
            var userInfo = (jwtToken as JwtSecurityToken)?.Claims?.ToList().ToUserInfo();

            if (userInfo == null)
                return null;
            
            return new UserTicket()
            {
                UserId = userInfo.UserId,
                Name = userInfo.Name,
                Roles = RolesParser.ToRoleTypes(userInfo.Roles),
                UserName = userInfo.UserName,
                PersonType = Enum.Parse<PersonType>(userInfo.PersonType),
                TokenId = userInfo.TokenId,
                TokenExpireAt = userInfo.TokenExpireAt,
                RefreshExpireAt = userInfo.RefreshExpireAt
            };
        }

    }
}
