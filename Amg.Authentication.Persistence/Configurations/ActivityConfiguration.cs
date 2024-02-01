using Amg.Authentication.DomainModel.Modules.UserActivities.Activities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Amg.Authentication.Persistence.Configurations
{
    public class ActivityConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id).ValueGeneratedNever();

            builder.ToTable("Activities");
        }
    }

    public class ChangePasswordActivityConfiguration : IEntityTypeConfiguration<ChangePasswordActivity>
    {
        public void Configure(EntityTypeBuilder<ChangePasswordActivity> builder)
        {
            builder.Property(i => i.Type).HasColumnName("Type");
        }
    }
    
    public class ChangeUserRolesActivityConfiguration : IEntityTypeConfiguration<ChangeUserRolesActivity>
    {
        public void Configure(EntityTypeBuilder<ChangeUserRolesActivity> builder)
        {
            builder.Property(i => i.Role).HasColumnName("Role");
        }
    }

    public class ChangeUserStatusActivityConfiguration : IEntityTypeConfiguration<ChangeUserStatusActivity>
    {
        public void Configure(EntityTypeBuilder<ChangeUserStatusActivity> builder)
        {
            builder.Property(i => i.IsActive).HasColumnName("IsActive");
        }
    }

    public class ChangeUserOtpStatusActivityConfiguration : IEntityTypeConfiguration<ChangeUserOtpStatusActivity>
    {
        public void Configure(EntityTypeBuilder<ChangeUserOtpStatusActivity> builder)
        {
            builder.Property(i => i.IsEnabled).HasColumnName("IsActive");
        }
    }

    public class ForgetPasswordActivityConfiguration : IEntityTypeConfiguration<ForgetPasswordActivity>
    {
        public void Configure(EntityTypeBuilder<ForgetPasswordActivity> builder)
        {
        }
    }

    public class SignInActivityConfiguration : IEntityTypeConfiguration<SignInActivity>
    {
        public void Configure(EntityTypeBuilder<SignInActivity> builder)
        {
            builder.Property(i => i.SignInType).HasColumnName("SignInType");
            builder.Property(i => i.ResultType).HasColumnName("ResultType");
        }
    }


    public class RegisterUserActivityConfiguration : IEntityTypeConfiguration<RegisterUserActivity>
    {
        public void Configure(EntityTypeBuilder<RegisterUserActivity> builder)
        {
            builder.Property(i => i.ByAdmin).HasColumnName("ByAdmin");
            builder.Property(i => i.Email).HasColumnName("Email");
            builder.Property(i => i.PhoneNumber).HasColumnName("PhoneNumber");
            builder.Property(i => i.Name).HasColumnName("Name");
        }
    }

    public class ResendCodeActivityConfiguration : IEntityTypeConfiguration<ResendCodeActivity>
    {
        public void Configure(EntityTypeBuilder<ResendCodeActivity> builder)
        {
            builder.Property(i => i.CodeType).HasColumnName("CodeType");
            builder.Property(i => i.PhoneNumber).HasColumnName("PhoneNumber");
        }
    }

    public class UpdateProfileInformationActivityConfiguration : IEntityTypeConfiguration<UpdateProfileActivity>
    {
        public void Configure(EntityTypeBuilder<UpdateProfileActivity> builder)
        {
            builder.Property(i => i.Email).HasColumnName("Email");
            builder.Property(i => i.Name).HasColumnName("Name");
            builder.Property(i => i.PhoneNumber).HasColumnName("PhoneNumber");
            builder.Property(i => i.TwoFactorEnabled).HasColumnName("TwoFactorEnabled");
        }
    }

    public class VerifyCodeActivityConfiguration : IEntityTypeConfiguration<VerifyCodeActivity>
    {
        public void Configure(EntityTypeBuilder<VerifyCodeActivity> builder)
        {
            builder.Property(i => i.CodeType).HasColumnName("CodeType");
        }
    }

}