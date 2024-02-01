using System;
using Amg.Authentication.DomainModel.SeedWorks.Base;

namespace Amg.Authentication.DomainModel.Modules.Permissions
{
    /// <summary>
    /// دسترسی
    /// </summary>
    public class Permission : AggregateRoot<Guid>
    {
        /// <inheritdoc />
        public Permission(string name, string description, string serviceName, string category) : base(Guid.NewGuid())
        {
            Name = name;
            Description = description;
            ServiceName = serviceName;
            Category = category;
            CreationDate = DateTime.Now;
        }

        /// <summary>
        /// نام
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// توضیحات
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// دسته بندی سطح دسترسی
        /// </summary>
        public string Category { get; private set; }

        /// <summary>
        /// نام سرویسی که از این پرمیژن استفاده می کند
        /// </summary>
        public string ServiceName { get; private set; }

        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreationDate { get; private set; }

        public void Update(string name, string description, string serviceName, string category)
        {
            Name = name;
            Description = description;
            ServiceName = serviceName;
            Category = category;
        }

        // FOR ORM !
        private Permission() : base(Guid.NewGuid()) { }
    }
}
