using System;
using System.Linq;
using System.Net;
using Amg.Authentication.Application.Contract.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using UAParser;
using ClientInfo = Amg.Authentication.Application.Contract.Dtos.ClientInfo;

namespace Amg.Authentication.Application.Services
{
    public class ClientInfoGrabber : IClientInfoGrabber
    {
        private static readonly Parser Parser = Parser.GetDefault();
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ClientInfoGrabber(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClientInfo GetClientInfo()
        {
            var clientInfo = new ClientInfo();
            var request = _httpContextAccessor.HttpContext.Request;

            try
            {
                // a.ammari:
                // در صورتی که سرویس پشت یک ریورس پروکسی باشد سعی می کنیم اطلاعات آی پی مبدا را استخراج کنیم
                // ریورس پروکسی باید اطلاعات آی پی مبدا را توسط یکی از هدر های استاندارد ارسال کند.

                var forwardedFor = request.Headers["X-Forwarded-For"];
                var forwarded = request.Headers["Forwarded"];
                var userAgent = request.Headers["User-Agent"];

                if (forwardedFor != StringValues.Empty && !string.IsNullOrEmpty(forwardedFor))
                {
                    clientInfo.IP = forwardedFor.ToString();
                }
                else if (forwarded != StringValues.Empty && !string.IsNullOrEmpty(forwarded))
                {
                    clientInfo.IP = forwarded.ToString()
                        .Split(';', StringSplitOptions.RemoveEmptyEntries)
                        .FirstOrDefault(i => i.Trim()
                            .StartsWith("for=", StringComparison.OrdinalIgnoreCase))?
                        .Split('=', StringSplitOptions.RemoveEmptyEntries)
                        .Last().Trim();
                }
                else
                {
                    clientInfo.IP = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
                }
                
                if (userAgent != StringValues.Empty && !string.IsNullOrEmpty(userAgent))
                {
                    var info = Parser.Parse(userAgent.ToString());
                    clientInfo.Agent = info.UA.ToString();
                    clientInfo.OS = info.OS.ToString();
                    clientInfo.Device = info.Device.ToString();
                }

                clientInfo.IP = IPEndPoint.Parse(clientInfo.IP).Address.ToString();
            }
            catch
            {
                // ignore
            }

            return clientInfo;
        }
    }
}
