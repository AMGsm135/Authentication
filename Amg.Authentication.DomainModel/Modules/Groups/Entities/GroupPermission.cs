using System;
using Amg.Authentication.DomainModel.Modules.Permissions;
using Amg.Authentication.DomainModel.SeedWorks.Base;

namespace Amg.Authentication.DomainModel.Modules.Groups.Entities
{
    public class GroupPermission : EntityBase<Guid>
    {
        public Group Group { get; private set; }
        public Permission Permission { get; private set; }

        public GroupPermission(Group group, Permission permission) : base(Guid.NewGuid())
        {
            Group = group;
            Permission = permission;
        }

        // for ORM
        private GroupPermission() : base(Guid.NewGuid())
        {
        }
    }
}
