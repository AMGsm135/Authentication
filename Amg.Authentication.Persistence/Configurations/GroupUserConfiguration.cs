using Amg.Authentication.DomainModel.Modules.Groups.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amg.Authentication.Persistence.Configurations
{
    public class GroupUserConfiguration : IEntityTypeConfiguration<GroupUser>
    {
        public void Configure(EntityTypeBuilder<GroupUser> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).ValueGeneratedNever();

            builder.HasOne(i => i.Group)
                .WithMany(i => i.GroupUsers)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("GroupUsers");
        }
    }
}