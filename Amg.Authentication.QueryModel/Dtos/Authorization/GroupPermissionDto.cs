using System;
using System.Collections.Generic;

namespace Amg.Authentication.QueryModel.Dtos.Authorization
{
    public class GroupPermissionDto
    {
        /// <summary>
        /// دسته بندی
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// سطح دسترسی های موجود در این دسته بندی
        /// </summary>
        public IList<GroupPermissionItemDto> Permissions { get; set; }
    }


    public class GroupPermissionItemDto
    {
        /// <summary>
        /// شناسه دسترسی
        /// </summary>
        public Guid PermissionId { get; set; }

        /// <summary>
        /// نام دسترسی
        /// </summary>
        public string PermissionName { get; set; }

        /// <summary>
        /// توضیحات دسترسی
        /// </summary>
        public string PermissionDescription { get; set; }

        /// <summary>
        /// آیا دسترسی را دارد ؟
        /// </summary>
        public bool HasPermission { get; set; }
    }

}
