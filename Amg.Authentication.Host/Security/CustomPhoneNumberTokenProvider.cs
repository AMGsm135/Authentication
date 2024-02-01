using System;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Amg.Authentication.Host.Security
{
    public class CustomPhoneNumberTokenProvider<TUser> : PhoneNumberTokenProvider<TUser>
    where TUser : IdentityUser<Guid>
    {
        private const int DigitsCount = 5;
        private static readonly int CodeGenerationExpireTime = 60;
        private static readonly int CodeValidationExpireTime = 240; // must dividable to CodeGenerationExpireTime

        public override async Task<string> GenerateAsync(string purpose, UserManager<TUser> manager, TUser user)
        {
            var securityToken = await manager.CreateSecurityTokenAsync(user);
            var userModifier = await GetUserModifierAsync(purpose, manager, user);
            return GenerateCode(securityToken, userModifier)
                .ToString($"D{DigitsCount}", CultureInfo.InvariantCulture);
        }

        public override async Task<bool> ValidateAsync(string purpose, string token, UserManager<TUser> manager, TUser user)
        {
            if (!int.TryParse(token, out var code))
                return false;
            var securityToken = await manager.CreateSecurityTokenAsync(user);
            var userModifier = await GetUserModifierAsync(purpose, manager, user);
            return securityToken != null && ValidateCode(securityToken, code, userModifier);
        }



        #region private Methods


        public static int GenerateCode(byte[] securityToken, string modifier = null)
        {
            if (securityToken == null)
                throw new ArgumentNullException(nameof(securityToken));

            var currentTimeStep = GetCurrentTimeStepNumber();
            using var hashAlgorithm = new HMACSHA1(securityToken);
            return ComputeTotp(hashAlgorithm, currentTimeStep, modifier);
        }

        public static bool ValidateCode(byte[] securityToken, int code, string modifier = null)
        {
            if (securityToken == null)
                throw new ArgumentNullException(nameof(securityToken));

            var currentTimeStep = GetCurrentTimeStepNumber();
            using var hashAlgorithm = new HMACSHA1(securityToken);
            var timeStep = (CodeValidationExpireTime / 2) / CodeGenerationExpireTime;
            for (var i = -timeStep; i <= timeStep; i++)
            {
                var computedTotp = ComputeTotp(hashAlgorithm, (ulong)((long)currentTimeStep + i), modifier);
                if (computedTotp == code)
                    return true;
            }

            return false;
        }
        
        private static int ComputeTotp(HashAlgorithm hashAlgorithm, ulong timeStepNumber, string modifier)
        {
            // add an optional modifier
            var timeStepAsBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((long)timeStepNumber));
            var hash = hashAlgorithm.ComputeHash(ApplyModifier(timeStepAsBytes, modifier));

            var offset = hash[^1] & 0xf;
            var binaryCode = (hash[offset] & 0x7f) << 24
                             | (hash[offset + 1] & 0xff) << 16
                             | (hash[offset + 2] & 0xff) << 8
                             | (hash[offset + 3] & 0xff);

            return binaryCode % (int)Math.Pow(10, DigitsCount);
        }

        private static byte[] ApplyModifier(byte[] input, string modifier)
        {
            if (string.IsNullOrEmpty(modifier))
                return input;

            var modifierBytes = Encoding.UTF8.GetBytes(modifier);
            var combined = new byte[checked(input.Length + modifierBytes.Length)];
            Buffer.BlockCopy(input, 0, combined, 0, input.Length);
            Buffer.BlockCopy(modifierBytes, 0, combined, input.Length, modifierBytes.Length);
            return combined;
        }

        private static ulong GetCurrentTimeStepNumber()
        {
            var delta = DateTime.UtcNow - (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc));
            return (ulong)((delta.Ticks / 10_000_000) / CodeGenerationExpireTime);
        }

        #endregion
    }





}
