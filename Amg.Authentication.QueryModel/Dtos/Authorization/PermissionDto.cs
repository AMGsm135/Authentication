using System;

namespace Amg.Authentication.QueryModel.Dtos.Authorization
{
    public class PermissionDto
    {
        /// <summary>
        /// شناسه
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// نام
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// توضیحات
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// نام سرویس
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// دسته بندی سطح دسترسی
        /// </summary>
        public string Category { get; set; }
    }
}
