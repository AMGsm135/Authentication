using Amg.Authentication.Infrastructure.Base;

namespace Amg.Authentication.Application.Contract.Dtos
{
    public class CaptchaValidation : ICaptchaValidation
    {
        /// <summary>
        /// summery
        /// </summary>
        /// <example> string </example>
        public string CaptchaId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <example> string </example>
        public string CaptchaCode { get; set; }
    }
}