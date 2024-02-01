using Amg.Authentication.DomainModel.Modules.Groups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amg.Authentication.Persistence.Configurations
{
    public class GroupConfiguration : IEntityTypeConfiguration<Group>
    {
        public void Configure(EntityTypeBuilder<Group> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).ValueGeneratedNever();

            builder.UsePropertyAccessMode(PropertyAccessMode.Field);
            builder.HasMany(i => i.GroupUsers)
                .WithOne(i => i.Group)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(i => i.GroupPermissions)
                .WithOne(i => i.Group)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Groups");
        }
    }
}