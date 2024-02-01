using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Infrastructure.Helpers;
using Amg.Authentication.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using SkiaSharp;
using SkiaSharp.QrCode;

namespace Amg.Authentication.Application.Services
{
    public class OtpAuthenticatorService : IOtpAuthenticatorService
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private readonly AuthSettings _authSettings;

        public OtpAuthenticatorService(IOptions<AuthSettings> authSettings)
        {
            _authSettings = authSettings.Value;
        }

        public byte[] GenerateActivationQrCode(string userAccount, string secretKey, int size)
        {
            if (string.IsNullOrWhiteSpace(userAccount))
                throw new ArgumentNullException(nameof(userAccount));

            var validUserAccount = RemoveWhitespace(Uri.EscapeUriString(userAccount));
            var encodedSecretKey = Base32Encoding.Encode(GetKeyBytes(secretKey));

            //  https://github.com/google/google-authenticator/wiki/Conflicting-Accounts
            // a.ammari : اضافه کردن عنوان صادر کننده برای سازگاری با نسخه های قبل
            var provisionUrl = string.IsNullOrWhiteSpace(_authSettings.OtpAuthenticator.Issuer) ?
                $"otpauth://totp/{validUserAccount}?secret={encodedSecretKey.Trim('=')}" :
                $"otpauth://totp/{UrlEncode(_authSettings.OtpAuthenticator.Issuer)}:{validUserAccount}?" +
                    $"secret={encodedSecretKey.Trim('=')}&issuer={UrlEncode(_authSettings.OtpAuthenticator.Issuer)}";

            return GenerateQrCodeUrl(provisionUrl, size);
        }

        public string GenerateOtpCode(string secretKey)
        {
            return GetCurrentPin(secretKey, GetCurrentCounter());
        }

        public bool ValidateTwoFactorPin(string secretKey, string otpCode)
        {
            return GetCurrentPins(secretKey, TimeSpan.FromSeconds(_authSettings.OtpAuthenticator.ClockDriftTolerance))
                .Any(c => c == otpCode);
        }






        private static byte[] GenerateQrCodeUrl(string provisionUrl, int size)
        {

            using var generator = new QRCodeGenerator();
            using var qrCodeData = generator.CreateQrCode(provisionUrl, ECCLevel.Q);
            using var surface = SKSurface.Create(new SKImageInfo(size, size));
            var canvas = surface.Canvas;
            canvas.Render(qrCodeData, size, size);

            using var image = surface.Snapshot();
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            using var stream = new MemoryStream();
            data.SaveTo(stream);

            stream.Seek(0, SeekOrigin.Begin);
            return stream.ToArray();
        }

        private static string RemoveWhitespace(string str) => new string(str.Where(c => !char.IsWhiteSpace(c)).ToArray());

        private static string UrlEncode(string value)
        {
            return Uri.EscapeDataString(value);
        }

        private long GetCurrentCounter() => GetCurrentCounter(DateTime.UtcNow, Epoch, 30);

        private long GetCurrentCounter(DateTime now, DateTime epoch, int timeStep) => (long)(now - epoch).TotalSeconds / timeStep;

        private string GetCurrentPin(string secretKey, long iterationNumber, int digits = 6)
        {
            var key = GetKeyBytes(secretKey);
            var counter = BitConverter.GetBytes(iterationNumber);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(counter);

            var hmac = new HMACSHA1(key);
            var hash = hmac.ComputeHash(counter);
            var offset = hash[^1] & 0xf;

            // Convert the 4 bytes into an integer, ignoring the sign.
            var binary =
                ((hash[offset] & 0x7f) << 24)
                | (hash[offset + 1] << 16)
                | (hash[offset + 2] << 8)
                | hash[offset + 3];

            var password = binary % (int)Math.Pow(10, digits);
            return password.ToString(new string('0', digits));
        }

        private string[] GetCurrentPins(string secretKey, TimeSpan timeTolerance)
        {
            var codes = new List<string>();
            var iterationCounter = GetCurrentCounter();
            var iterationOffset = 0;

            if (timeTolerance.TotalSeconds > 30)
            {
                iterationOffset = Convert.ToInt32(timeTolerance.TotalSeconds / 30.00) / 2;
            }

            var iterationStart = iterationCounter - iterationOffset;
            var iterationEnd = iterationCounter + iterationOffset;

            for (var counter = iterationStart; counter <= iterationEnd; counter++)
            {
                codes.Add(GetCurrentPin(secretKey, counter));
            }

            return codes.ToArray();
        }

        private byte[] GetKeyBytes(string secretKey)
        {
            using var sha = SHA1.Create();
            return sha.ComputeHash(Encoding.UTF8.GetBytes(secretKey));
        }

    }
}
