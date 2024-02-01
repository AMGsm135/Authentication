using System.Collections.Generic;

namespace Amg.Authentication.QueryModel.Dtos.Authorization
{
    public class UserGroupPermissionsDto
    {
        /// <summary>
        /// نام گروه کاربر
        /// </summary>
        public string FirstGroupName { get; set; }

        /// <summary>
        /// نام دسترسی های کاربر
        /// </summary>
        public IList<string> PermissionsNames { get; set; }
    }
}
