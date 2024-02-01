namespace Amg.Authentication.Application.Contract
{
    public static class EmailMessages
    {
        public static string ActivationCodeSubject() => $"کد تایید حساب کاربری"; 
        public static string ActivationCodeBody(string code) => $"کد تایید شما عبارت است از:\r\n{code}";
        
        
        public static string TwoFactorCodeSubject() => $"کد تایید دوعاملی";
        public static string TwoFactorCodeBody(string code) => $"کد تایید شما عبارت است از:\r\n{code}";


        public static string ForgetPasswordCodeSubject() => $"کد تایید فراموشی رمز عبور";
        public static string ForgetPasswordCodeBody(string code) => $"کد تغییر رمز عبور شما عبارت است از:\r\n{code}";


        public static string ResetPasswordByAdminSubject() => $"ارسال رمز عبور جدید";
        public static string ResetPasswordByAdminBody(string code) => $"مشتری گرامی، رمز عبور جدید شما برابر است با:\r\n{code}";

    }
}
