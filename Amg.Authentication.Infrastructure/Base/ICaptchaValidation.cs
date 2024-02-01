namespace Amg.Authentication.Infrastructure.Base
{
    public interface ICaptchaValidation
    {
        public string CaptchaId { get; set; }
        public string CaptchaCode { get; set; }
    }
}
