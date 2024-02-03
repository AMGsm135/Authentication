using System;
using System.Collections.Generic;
using Amg.Authentication.Infrastructure.Enums;

namespace Amg.Authentication.QueryModel.Dtos.Accounting
{
    public class UserDto
    {
        /// <summary>
        /// شناسه
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// نام کاربری
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// نام
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// شماره موبایل
        /// </summary>
        public string PhoneNumber { get; set; }

        /// <summary>
        /// ایمیل
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// آیا فعال است ؟
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// نقش های کاربر
        /// </summary>
        public IList<string> Roles { get; set; }

        /// <summary>
        /// نیاز به تایید دو عاملی است؟
        /// </summary>
        public bool TwoFactorEnabled { get; set; }

        /// <summary>
        /// تاریخ ثبت
        /// </summary>
        public DateTime RegisterDateTime { get; set; }

        /// <summary>
        /// نوع شخص
        /// </summary>
        public PersonType PersonType { get; set; }

        /// <summary>
        /// آیا کاربر موقتا مسدود شده است؟
        /// </summary>
        public bool LockoutEnabled { get; set; }

        /// <summary>
        /// شهر
        /// </summary>
        public string City { get; set; }


        /// <summary>
        /// استان
        /// </summary>
        public string Province { get; set; }
    }
}
