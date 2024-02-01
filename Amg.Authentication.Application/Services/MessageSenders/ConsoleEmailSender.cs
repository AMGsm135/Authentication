using System;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Amg.Authentication.Application.Services.MessageSenders
{
    /// <summary>
    /// سرویس ارسال ایمیل در کنسول
    /// </summary>
    public class ConsoleEmailSender : IEmailSender

    {
        private readonly NotificationSettings _notificationSettings;
        public ConsoleEmailSender(IOptions<NotificationSettings> notificationSettings)
        {
            _notificationSettings = notificationSettings.Value;
        }


        public Task SendEmailAsync(string recipient, string subject, string message)
        {
            try
            {
                var messageText = CreateMessage(message);
                Console.WriteLine($"-------------------- Email --------------------");
                Console.WriteLine($"TO : {recipient}");
                Console.WriteLine($"Subject : {subject}");
                Console.WriteLine(messageText);
                Console.WriteLine($"-----------------------------------------------");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while Sending Email : " + ex.Message);
            }
            return Task.CompletedTask;
        }

        private string CreateMessage(string message)
        {
            return message;
        }
    }
}
