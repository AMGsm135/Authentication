using Microsoft.AspNetCore.Identity;

namespace Amg.Authentication.Host.SeedWorks
{
    public class PersianIdentityErrorDescriber : IdentityErrorDescriber
    {

        public override IdentityError DefaultError() => new IdentityError
            {Code = nameof(DefaultError), Description = "خطا در درسترسی به اطلاعات کاربری"};


    public override IdentityError ConcurrencyFailure() => new IdentityError
        { Code = nameof(ConcurrencyFailure), Description = "خطا در بررسی اطلاعات کاربری. لطفا مجددا تلاش کنید" };

        public override IdentityError PasswordMismatch() => new IdentityError
            { Code = nameof(PasswordMismatch), Description = "رمز عبور نادرست است" };

        public override IdentityError InvalidToken() => new IdentityError
            { Code = nameof(InvalidToken), Description = "توکن نامعتبر است" };

        public override IdentityError RecoveryCodeRedemptionFailed() => new IdentityError
            { Code = nameof(RecoveryCodeRedemptionFailed), Description = "کد بازیابی نامعتبر است" };

        public override IdentityError LoginAlreadyAssociated() => new IdentityError
            { Code = nameof(LoginAlreadyAssociated), Description = "کاربر هم اکنون به سیستم وارد شده است" };

        public override IdentityError InvalidUserName(string userName) => new IdentityError
            { Code = nameof(InvalidUserName), Description = "نام کاربری اشتباه است" };

        public override IdentityError InvalidEmail(string email) => new IdentityError
            { Code = nameof(InvalidEmail), Description = "ایمیل نامعتبر است" };

        public override IdentityError DuplicateUserName(string userName) => new IdentityError
            { Code = nameof(DuplicateUserName), Description = "نام کاربری تکراری است" };

        public override IdentityError DuplicateEmail(string email) => new IdentityError
            { Code = nameof(DuplicateEmail), Description = "ایمیل تکراری است" };

        public override IdentityError InvalidRoleName(string role) => new IdentityError
            { Code = nameof(DefaultError), Description = "خطا در درسترسی به اطلاعات کاربری" };

        public override IdentityError DuplicateRoleName(string role) => new IdentityError
            { Code = nameof(DuplicateRoleName), Description = $"نقش {role} تکراری است" };

        public override IdentityError UserAlreadyHasPassword() => new IdentityError
            { Code = nameof(UserAlreadyHasPassword), Description = "کاربر هم اکنون دارای رمز عبور می باشد" };

        public override IdentityError UserLockoutNotEnabled() => new IdentityError
            { Code = nameof(UserLockoutNotEnabled), Description = "مسدود سازی برای کاربر فعال نیست" };

        public override IdentityError UserAlreadyInRole(string role) => new IdentityError
            { Code = nameof(UserAlreadyInRole), Description = $"نقش {role} هم اکنون به این کاربر انتساب داده شده است" };

        public override IdentityError UserNotInRole(string role) => new IdentityError
            { Code = nameof(UserNotInRole), Description = $"نقش {role} هم اکنون به این کاربر انتساب داده نشده است" };

        public override IdentityError PasswordTooShort(int length) => new IdentityError
            { Code = nameof(PasswordTooShort), Description = "رمز عبور کوتاه است" };

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) => new IdentityError
            { Code = nameof(PasswordRequiresUniqueChars), Description = $"رمز عبور حداقل به {uniqueChars} کاراکتر منحصر به فرد نیاز دارد" };

        public override IdentityError PasswordRequiresNonAlphanumeric() => new IdentityError
            { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "رمز عبور باید حداقل دارای یک یا چند کاراکتر غیر عدد و حروف باشد" };

        public override IdentityError PasswordRequiresDigit() => new IdentityError
            { Code = nameof(PasswordRequiresDigit), Description = "رمز عبور باید حداقل دارای یک یا چند کاراکتر عددی باشد" };

        public override IdentityError PasswordRequiresLower() => new IdentityError
            { Code = nameof(PasswordRequiresLower), Description = "رمز عبور باید حداقل دارای یک یا چند حرف کوچک باشد" };

        public override IdentityError PasswordRequiresUpper() => new IdentityError
            { Code = nameof(PasswordRequiresUpper), Description = "رمز عبور باید حداقل دارای یک یا چند حرف بزرگ باشد" };



        private IdentityError CreateError(string code, string description)
        {
            return new IdentityError()
            {
                Code = code,
                Description = description
            };
        }
    }

}