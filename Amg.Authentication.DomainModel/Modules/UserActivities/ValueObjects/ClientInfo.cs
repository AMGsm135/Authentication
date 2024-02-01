using Amg.Authentication.DomainModel.SeedWorks.Base;

namespace Amg.Authentication.DomainModel.Modules.UserActivities.ValueObjects
{
    public class ClientInfo : ValueObject<ClientInfo>
    {
        public ClientInfo(string ip, string os, string device, string agent)
        {
            IP = ip;
            OS = os;
            Device = device;
            Agent = agent;
        }

        /// <summary>
        /// آی پی کاربر
        /// </summary>
        public string IP { get; private set; }

        /// <summary>
        /// سیستم عامل کاربر
        /// </summary>
        public string OS { get; private set; }

        /// <summary>
        /// دستگاه کاربر
        /// </summary>
        public string Device { get; private set; }

        /// <summary>
        /// مرورگر یا نرم افزاری که توسط آن درخواست ارسال شده است
        /// </summary>
        public string Agent { get; private set; }


        protected override void Validate()
        {
        }

        public static ClientInfo Empty()
        {
            return new ClientInfo();
        }


        // for ORM
        private ClientInfo()
        {

        }
    }
}
