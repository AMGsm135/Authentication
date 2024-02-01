using System;
using Amg.Authentication.DomainModel.SeedWorks.Base;

namespace Amg.Authentication.DomainModel.Modules.UserActivities.Activities
{
    public abstract class Activity : EntityBase<Guid>
    {
        protected Activity(bool isAdministrative) : base(Guid.NewGuid())
        {
            IsAdministrative = isAdministrative;
        }

        /// <summary>
        /// آیا این عمل مدیریتی بوده است؟
        /// در این صورت لاگ برای کاربر نمایش داده نمی شود و فقط برای مدیر قابل نمایش است
        /// </summary>
        public bool IsAdministrative { get; private set; }

        public abstract string GetDescription();

        protected override void Validate()
        {
        }

    }
}