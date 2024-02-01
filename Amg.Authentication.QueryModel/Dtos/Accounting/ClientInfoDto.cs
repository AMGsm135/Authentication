namespace Amg.Authentication.QueryModel.Dtos.Accounting
{
    public class ClientInfoDto
    {
        /// <summary>
        /// آی پی کاربر
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// سیستم عامل کاربر
        /// </summary>
        public string OS { get; set; }

        /// <summary>
        /// دستگاه کاربر
        /// </summary>
        public string Device { get; set; }

        /// <summary>
        /// مرورگر یا نرم افزاری که توسط آن درخواست ارسال شده است
        /// </summary>
        public string Agent { get; set; }
    }
}
