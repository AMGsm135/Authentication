using Amg.Authentication.DomainModel.Modules.UserActivities;
using Amg.Authentication.DomainModel.Modules.UserActivities.Activities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amg.Authentication.Persistence.Configurations
{
    public class UserActivityConfiguration : IEntityTypeConfiguration<UserActivity>
    {
        public void Configure(EntityTypeBuilder<UserActivity> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).ValueGeneratedNever();

            builder.HasOne(i => i.Activity)
                .WithOne().HasForeignKey<Activity>("UserActivityId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.OwnsOne(i => i.ClientInfo);

            builder.ToTable("UserActivities");
        }
    }
}
