using System;
using System.Net.Http;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Application.Services.CashServices;
using Amg.Authentication.Infrastructure.Settings;
using Kavenegar.Exceptions;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using static System.Net.WebRequestMethods;

namespace Amg.Authentication.Application.Services.MessageSenders
{
    /// <summary>
    /// سرویس ارسال پیامک توسط مایکروسرویس
    /// Notification Service
    /// </summary>
    public class NotificationServiceSmsSender : ISmsSender
    {
        private readonly HttpClient _client;
        private readonly NotificationSettings _notificationSettings;
        private readonly ICacheService _cacheService;

        public NotificationServiceSmsSender(IHttpClientFactory clientFactory, IOptions<NotificationSettings> notificationSettings, RedisClient redisClient, ICacheService cacheService)
        {
            _notificationSettings = notificationSettings.Value;
            _client = clientFactory.CreateClient(Constants.NotificationHttpClient);
            _cacheService = cacheService;
        }

        public Task SendSmsAsync(string phoneNumber, string message)
        {
            throw new NotImplementedException();
        }

        private void AddToCache(string phoneNumber, string code)
        {
            _cacheService.SetData(phoneNumber, code,
                TimeSpan.FromSeconds(_notificationSettings.Sms.MinimumResendTime));
        }

        public async Task<(bool isSuccess, string message)> SendSmsAsyncWithResult(string phoneNumber, string message)
        {
            try
            {
                if (_notificationSettings.Sms.DevelopmentMode)
                {
                    _cacheService.SetData(phoneNumber, _notificationSettings.Sms.DevelopmentCode, TimeSpan.FromSeconds(_notificationSettings.Sms.MinimumResendTime));
                    return (true, string.Empty);
                }

                string url = $"https://api.kavenegar.com/v1/3147625A597976776C417076306B6B7A6A63556E4B59546F552F446E3479765137524B735934754C4B55303D/verify/lookup.json?receptor={phoneNumber}&token={message}&template=verify";
           
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.PostAsync(url, null);

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("SMS Exception Error Occurred: " + response.ReasonPhrase + "," + response.StatusCode + "StatusCode : " + response.IsSuccessStatusCode.ToString());
                        return (false, "ارتباط با  پنل پیامکی برقرار نشد .");
                    }

                    _cacheService.SetData(phoneNumber, message, TimeSpan.FromSeconds(_notificationSettings.Sms.MinimumResendTime));

                    return (true, string.Empty);

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error Message : " + ex.Message + "Inner Exception : " + ex.InnerException.Message);
                return (false, "به هنگام ارسال پیامک اختلالی رخ داده است لطفا  مجددا تلاش کنید");
            }
        }
    }
}
