using Amg.Authentication.DomainModel.Modules.Groups.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amg.Authentication.Persistence.Configurations
{
    public class GroupPermissionConfiguration : IEntityTypeConfiguration<GroupPermission>
    {
        public void Configure(EntityTypeBuilder<GroupPermission> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).ValueGeneratedNever();

            builder.HasOne(i => i.Permission)
                .WithMany().OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(i => i.Group)
                .WithMany(i => i.GroupPermissions)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("GroupPermissions");
        }
    }
}