namespace Amg.Authentication.Infrastructure.Settings
{
    public class NotificationSettings
    {
        public SmsSettings Sms { get; set; }
        public EmailSettings Email { get; set; }
    }

    public class SmsSettings
    {
        public bool DevelopmentMode { get; set; }
        public string DevelopmentCode { get; set; }
        public string HeaderText { get; set; }
        public string TrailerText { get; set; }
        public int MinimumResendTime { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Domain { get; set; }
        public string Number { get; set; }
        public string Template { get; set; }
        public string ApiKey { get; set; }
    }

    public class EmailSettings
    {
        public string SmtpHost { get; set; }
        public int SmtpPort { get; set; }
        public string SenderMail { get; set; }
        public string Password { get; set; }
    }
}
