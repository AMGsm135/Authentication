using System;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Amg.Authentication.Application.Services.MessageSenders
{
    /// <summary>
    /// سرویس ارسال پیامک در کنسول
    /// </summary>
    public class ConsoleSmsSender : ISmsSender
    {
        private readonly NotificationSettings _notificationSettings;

        public ConsoleSmsSender(IOptions<NotificationSettings> notificationSettings)
        {
            _notificationSettings = notificationSettings.Value;
        }


        public Task SendSmsAsync(string phoneNumber, string message)
        {
            try
            {
                var messageText = CreateMessage(message, _notificationSettings.Sms.HeaderText, _notificationSettings.Sms.TrailerText);
                Console.WriteLine($"-------------------- SMS --------------------");
                Console.WriteLine($"TO : {phoneNumber}");
                Console.WriteLine(messageText);
                Console.WriteLine($"---------------------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while Sending SMS : " + ex.Message);
            }
            return Task.CompletedTask;

        }


        private string CreateMessage(string message, string header, string trailer)
        {
            return $@"*{header}*\r\n{message}\r\n{trailer}";

        }

        Task<(bool, string)> ISmsSender.SendSmsAsyncWithResult(string phoneNumber, string message)
        {
            throw new NotImplementedException();
        }
    }
}
