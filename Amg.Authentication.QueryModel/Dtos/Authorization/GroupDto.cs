using System;

namespace Amg.Authentication.QueryModel.Dtos.Authorization
{
    public class GroupDto
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
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        /// تعداد کاربران
        /// </summary>
        public int TotalUsers { get; set; }
    }
}
