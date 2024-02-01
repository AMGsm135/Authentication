using System;
using System.Threading.Tasks;
using Amg.Authentication.Application.Contract.Services;
using Amg.Authentication.Infrastructure.Settings;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Amg.Authentication.Application.Services.MessageSenders
{
    /// <summary>
    /// سرویس ارسال ایمیل توسط پروتکل SMTP
    /// </summary>
    public class DefaultSmtpEmailSender : IEmailSender

    {
        private readonly NotificationSettings _notificationSettings;
        public DefaultSmtpEmailSender(IOptions<NotificationSettings> notificationSettings)
        {
            _notificationSettings = notificationSettings.Value;
        }


        public async Task SendEmailAsync(string recipient, string subject, string message)
        {
            try
            {
                var msg = new MimeMessage();
                msg.From.Add(new MailboxAddress(_notificationSettings.Email.SenderMail, _notificationSettings.Email.SenderMail));
                msg.To.Add(MailboxAddress.Parse(recipient));
                msg.Subject = subject;
                msg.Body = new TextPart(TextFormat.Html)
                {
                    Text = CreateMessage(message)
                };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_notificationSettings.Email.SmtpHost, _notificationSettings.Email.SmtpPort, true);
                await smtp.AuthenticateAsync(_notificationSettings.Email.SenderMail, _notificationSettings.Email.Password);
                await smtp.SendAsync(msg);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while Sending Email : " + ex.Message);

            }
        }

        private string CreateMessage(string message)
        {
            return message;
        }
    }
}
