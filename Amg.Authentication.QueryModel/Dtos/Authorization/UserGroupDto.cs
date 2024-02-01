using System;

namespace Amg.Authentication.QueryModel.Dtos.Authorization
{
    public class UserGroupDto
    {
        /// <summary>
        /// شناسه گروه
        /// </summary>
        public Guid GroupId { get; set; }

        /// <summary>
        /// نام گروه
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// آیا کاربر داخل این گروه هست
        /// </summary>
        public bool IsInGroup { get; set; }
    }
}
