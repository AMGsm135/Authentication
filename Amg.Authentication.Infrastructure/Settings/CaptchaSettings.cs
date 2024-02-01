namespace Amg.Authentication.Infrastructure.Settings
{
    public class CaptchaSettings
    {
        /// <summary>
        /// آیا کپچا بررسی شود؟
        /// </summary>
        public bool IsEnabled { get; set; }
        
        /// <summary>
        /// زمان اعتبار کد کپچا
        /// </summary>
        public int Timeout { get; set; }
        
        /// <summary>
        /// حالت توسعه، برای ورود کپچای ثابت تستی
        /// </summary>
        public bool DevelopmentMode { get; set; }
        
        /// <summary>
        /// شناسه کپچای تستی
        /// </summary>
        public string DevelopmentCaptchaId { get; set; }
        
        /// <summary>
        /// کد کپچای تستی
        /// </summary>
        public string DevelopmentCaptchaCode { get; set; }

    }
}
