using System;
using Amg.Authentication.DomainModel.Modules.UserActivities.Activities;
using Amg.Authentication.DomainModel.Modules.UserActivities.ValueObjects;
using Amg.Authentication.DomainModel.SeedWorks.Base;

namespace Amg.Authentication.DomainModel.Modules.UserActivities
{
    public class UserActivity : AggregateRoot<Guid>
    {
        public UserActivity(Guid userId, bool isSuccess, DateTime activityDateTime,
            Activity activity, ClientInfo clientInfo, string errorMessage)
            : base(Guid.NewGuid())
        {
            UserId = userId;
            IsSuccess = isSuccess;
            if (!isSuccess)
                ErrorExceptionMessage = errorMessage;
            ActivityDateTime = activityDateTime;
            Activity = activity;
            ClientInfo = clientInfo;
        }

        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public Guid UserId { get; private set; }    

        /// <summary>
        /// آیا عملیات موفقیت آمیز است؟
        /// </summary>
        public bool IsSuccess { get; private set; }

        /// <summary>
        /// تاریخ عملیات
        /// </summary>
        public DateTime ActivityDateTime { get; private set; }

        /// <summary>
        /// عملیات
        /// </summary>
        public Activity Activity { get; private set; }

        /// <summary>
        /// مشخصات کلاینت درخواست دهنده
        /// </summary>
        public ClientInfo ClientInfo { get; private set; }

        /// <summary>
        /// if isSuccess was False so the field will be completed with the reosen of the exception raise
        /// </summary>
        public string ErrorExceptionMessage { get; set; }

        // for ORM
        private UserActivity() : base(Guid.NewGuid())
        {

        }

    }
}
