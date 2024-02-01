namespace Amg.Authentication.Application.Contract
{
    public static class SmsMessages
    {
        public static string ActivationCode(string code) => $"کد تایید شما عبارت است از:\r\n{code}";
        
        public static string TwoFactorCode(string code) => $"کد تایید شما عبارت است از:\r\n{code}";

        public static string ForgetPasswordCode(string code) => $"کد تغییر رمز عبور شما عبارت است از:\r\n{code}";

        public static string ResetPasswordByAdmin(string code) => $"مشتری گرامی، رمز عبور جدید شما برابر است با:\r\n{code}";
    }
}
