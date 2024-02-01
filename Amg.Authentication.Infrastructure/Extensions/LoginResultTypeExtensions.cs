using Amg.Authentication.Infrastructure.Enums;

namespace Amg.Authentication.Infrastructure.Extensions
{
    public static class LoginResultTypeExtensions
    {
        public static string GetResultMessage(this SignInResultType signInResultType)
        {
            return signInResultType switch
            {
                SignInResultType.InvalidRequest => "درخواست نامعتبر است.",
                SignInResultType.InvalidCredentials => "نام کاربری یا رمز عبور اشتباه است.",
                SignInResultType.UserIsInactive => "حساب کاربری موجود نیست و یا غیر فعال شده است.",
                SignInResultType.UserIsLockedOut => "حساب کاربری به طور موقت مسدود شده است.",
                SignInResultType.ActivationNeeded => "حساب کاربری هنوز فعال نشده است.",
                SignInResultType.TwoFactorCodeNeeded => "برای ورود نیاز به کد تایید دو عاملی می باشد.",
                SignInResultType.TwoFactorCodeInvalid => "کد تایید دو عاملی نامعتبر است.",
                SignInResultType.LoginSuccessful => "ورود با موفقیت انجام شد.",
                _ => "خطا در بررسی اطلاعات حساب کاربری."
            };
        }
    }
}
