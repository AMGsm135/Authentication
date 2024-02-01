using System;
using Amg.Authentication.Application.Events.UserActivities.Items;

namespace Amg.Authentication.Application.Events.UserActivities.Base
{
    public class UserActivityEvent : IIntegrationEvent
    {
        protected UserActivityEvent()
        {
            EventId = Guid.NewGuid();
        }

        /// <summary>
        /// شناسه رویداد
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// شناسه کاربر
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// آیا عملیات موفقیت آمیز است؟
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// آیا عملیات موفقیت آمیز است؟
        /// </summary>
        public string ErrorExceptionMessage { get; set; }

        /// <summary>
        /// اطلاعات کلاینت
        /// </summary>
        public ClientInfo ClientInfo { get; set; }

       
    }

    public static class UserActivityEventExtention
    {
        /// <summary>
        /// if the IsSuccess field will be false so the ErrorExceptionMessage field will be completed automatically
        /// </summary>
        public static UserActivityEvent FillTheErrorExceptionMessageIfIsSuccessFieldWasFalse(this UserActivityEvent @event, string exceptionErrorMessage)
        {
            if (!@event.IsSuccess)
            {
                @event.ErrorExceptionMessage = exceptionErrorMessage;
            }

            return @event;
        }
    }
}
