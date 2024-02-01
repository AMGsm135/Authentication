using System;
using Amg.Authentication.DomainModel.SeedWorks.Base;

namespace Amg.Authentication.DomainModel.Modules.Groups.Entities
{
    public class GroupUser : EntityBase<Guid>
    {
        public Group Group { get; private set; }
        public Guid UserId { get; private set; }

        public GroupUser(Group group, Guid userId) : base(Guid.NewGuid())
        {
            Group = group;
            UserId = userId;
        }

        // for ORM
        private GroupUser() : base(Guid.NewGuid())
        {
        }
    }
}
