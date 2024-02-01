using System;
using System.Net.Http;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Infrastructure.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Amg.Authentication.Application.Services.MessageSenders
{
    /// <summary>
    /// دکوراتور ایجاد محدودیت در ارسال پیامک
    /// </summary>
    public class SmsSenderThrottleLimitingDecorator : ISmsSender
    {
        private readonly ISmsSender _smsSender;
        private readonly IDatabase _cacheDatabase;
        private const int DatabaseNumber = 3;
        private readonly NotificationSettings _notificationSettings;

        public SmsSenderThrottleLimitingDecorator(ISmsSender smsSender, RedisClient redisClient,
            IOptions<NotificationSettings> notificationSettings)
        {
            _smsSender = smsSender;
            _cacheDatabase = redisClient.Database(DatabaseNumber);
            _notificationSettings = notificationSettings.Value;
        }

        public async Task SendSmsAsync(string phoneNumber, string message)
        {
            if (ExistsInCache(phoneNumber))
                return;

            await _smsSender.SendSmsAsync(phoneNumber, message);

            AddToCache(phoneNumber,message);
        }

        private bool ExistsInCache(string phoneNumber)
        {
            return _cacheDatabase.KeyExists(phoneNumber);
        }

        private void AddToCache(string phoneNumber,string code)
        {
            _cacheDatabase.StringSet(phoneNumber, code,
                TimeSpan.FromSeconds(_notificationSettings.Sms.MinimumResendTime));
        }

        async Task<(bool, string)> ISmsSender.SendSmsAsyncWithResult(string phoneNumber, string message)
        {
            if (ExistsInCache(phoneNumber))
                return (false, "پیامک تاییده شماره همراه قبلا ارسال شده است.");

            try
            {
                string apiUrl = _notificationSettings.Sms.ApiKey;
                string receptor = phoneNumber;
                string token = message;
                string template = _notificationSettings.Sms.Template;

                // Create the URL with query parameters
                string url = $"{apiUrl}?receptor={receptor}&token={token}&template={template}";

                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.PostAsync(url, null);

                    if (response.IsSuccessStatusCode)
                        return (true, string.Empty);
                    else
                        return (false, "SMS Exception Error Occurred: " + response.ReasonPhrase + "," + response.StatusCode + "StatusCode : " + response.IsSuccessStatusCode.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while Sending SMS : " + ex.Message);
                return (false, "Error Message : " + ex.Message + "Inner Exception : " + ex.InnerException.Message);
            }
            finally
            {
                AddToCache(phoneNumber, message);
            }
        }
    }
}
