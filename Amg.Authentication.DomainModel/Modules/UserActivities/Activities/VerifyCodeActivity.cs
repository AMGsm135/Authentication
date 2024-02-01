using Amg.Authentication.Infrastructure.Enums.UserActivities;
using Amg.Authentication.Infrastructure.Extensions;

namespace Amg.Authentication.DomainModel.Modules.UserActivities.Activities
{
    public class VerifyCodeActivity : Activity
    {
        public SmsCodeType CodeType { get; private set; }

        public override string GetDescription() => $"اعتبار سنجی {CodeType.GetDescription()}";

        public VerifyCodeActivity(SmsCodeType codeType) : base(false)
        {
            CodeType = codeType;
        }

        // for ORM
        private VerifyCodeActivity() : base(false)
        {
        }
    }
}