using System;
using Amg.Authentication.DomainModel.SeedWorks.Base;
using Amg.Authentication.Infrastructure.Enums;
using Microsoft.AspNetCore.Identity;

namespace Amg.Authentication.DomainModel.Modules.Users
{
    public class User : IdentityUser<Guid>, IAggregateRoot
    {
        /// <inheritdoc />
        public User(string userName, string firstName, string lastName, PersonType type, string provider, string city, string province) : base(userName)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.RegisterDateTime = DateTime.Now;
            this.PersonType = type;
            this.Provider = provider;
            this.IsActive = true;
            this.City = city;
            this.Province = province;
            this.Status = RegisteryStatus.Created;
        }

        /// <summary>
        /// نام
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// نام خانوادگی
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// شهر
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// استان
        /// </summary>
        public string Province { get; set; }

        /// <summary>
        /// تاریخ ثبت نام
        /// </summary>
        public DateTime RegisterDateTime { get; set; }

        /// <summary>
        /// آیا غیرفعال شده است ؟
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// ارائه دهنده احراز هویت خارجی مثل گوگل
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// نوع کاربر
        /// </summary>
        public PersonType PersonType { get; set; }

        /// <summary>
        /// رمز قبلی کاربر
        /// </summary>
        public string OldPassword { get; set; }

        /// <summary>
        /// آیا کد یک بارمصرف فعال است؟
        /// </summary>
        public bool OtpEnabled { get; set; }

        /// <summary>
        /// رمز تولید کد یک بار مصرف
        /// </summary>
        public string OtpSecretCode { get; set; }


        public RegisteryStatus Status { get; set; }


        public void Activate() => IsActive = true;

        public void DeActivate() => IsActive = false;

        public object GetId() => Id;

        // for ORM
        private User() { }

    }
}
