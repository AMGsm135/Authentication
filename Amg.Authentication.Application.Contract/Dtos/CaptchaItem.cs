using System;

namespace Amg.Authentication.Application.Contract.Dtos
{
    public class CaptchaItem
    {
        /// <summary>
        /// Id
        /// </summary>
        /// <example> string id value </example>
        public string CaptchaId { get; set; }

        /// <summary>
        /// Image
        /// </summary>
        /// <example> byte array value</example>
        public byte[] CaptchaImage { get; set; }

    }
}