using System;
using System.Collections.Generic;
using System.Linq;
using Amg.Authentication.DomainModel.Modules.Groups.Entities;
using Amg.Authentication.DomainModel.Modules.Permissions;
using Amg.Authentication.DomainModel.SeedWorks.Base;

namespace Amg.Authentication.DomainModel.Modules.Groups
{
    /// <summary>
    /// گروه
    /// </summary>
    public class Group : AggregateRoot<Guid>
    {
        private readonly List<GroupPermission> _groupPermissions = new List<GroupPermission>();
        private readonly List<GroupUser> _groupUsers = new List<GroupUser>();
        
        public Group(string name) : base(Guid.NewGuid())
        {
            Name = name;
            CreationDate = DateTime.Now;
        }

        /// <summary>
        /// نام
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// تاریخ ایجاد
        /// </summary>
        public DateTime CreationDate { get; private set; }

        public IReadOnlyList<GroupUser> GroupUsers => _groupUsers.AsReadOnly();

        public IReadOnlyList<GroupPermission> GroupPermissions => _groupPermissions.AsReadOnly();

        public void AddPermission(Permission permission) =>
            _groupPermissions.Add(new GroupPermission(this, permission));

        public void AddPermissions(IEnumerable<Permission> permissions) =>
            _groupPermissions.AddRange(permissions.Select(p => new GroupPermission(this, p)));

        public void RemoveAllPermissions() =>
            _groupPermissions.Clear();

        public void AddUser(Guid userId) =>
            _groupUsers.Add(new GroupUser(this, userId));

        public void RemoveUser(GroupUser groupUser) =>
            _groupUsers.Remove(groupUser);

        public void Update(string name) => Name = name;

        // for ORM
        private Group() : base(Guid.NewGuid())
        {
        }
    }
}
