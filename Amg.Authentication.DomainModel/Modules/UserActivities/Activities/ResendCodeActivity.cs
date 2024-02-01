using Amg.Authentication.Infrastructure.Enums.UserActivities;
using Amg.Authentication.Infrastructure.Extensions;

namespace Amg.Authentication.DomainModel.Modules.UserActivities.Activities
{
    public class ResendCodeActivity : Activity
    {
        public string PhoneNumber { get; private set; }
        
        public SmsCodeType CodeType { get; private set; }

        public override string GetDescription() => $"ارسال {CodeType.GetDescription()} به شماره {PhoneNumber}";

        public ResendCodeActivity(SmsCodeType codeType, string phoneNumber) : base(false)
        {
            CodeType = codeType;
            PhoneNumber = phoneNumber;
        }

        // for ORM
        private ResendCodeActivity() : base(false)
        {
        }
    }
}