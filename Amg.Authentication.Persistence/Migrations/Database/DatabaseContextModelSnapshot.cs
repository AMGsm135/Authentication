﻿// <auto-generated />
using System;
using Amg.Authentication.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Amg.Authentication.Persistence.Migrations.Database
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.22")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.Groups.Entities.GroupPermission", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("PermissionId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("PermissionId");

                    b.ToTable("GroupPermissions", (string)null);
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.Groups.Entities.GroupUser", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("GroupId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.ToTable("GroupUsers", (string)null);
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.Groups.Group", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Groups", (string)null);
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.Permissions.Permission", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Category")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("CreationDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ServiceName")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Permissions", (string)null);
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.Activity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAdministrative")
                        .HasColumnType("bit");

                    b.Property<Guid?>("UserActivityId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("UserActivityId")
                        .IsUnique()
                        .HasFilter("[UserActivityId] IS NOT NULL");

                    b.ToTable("Activities", (string)null);

                    b.HasDiscriminator<string>("Discriminator").HasValue("Activity");
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.UserActivities.UserActivity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ActivityDateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("ErrorExceptionMessage")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsSuccess")
                        .HasColumnType("bit");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.ToTable("UserActivities", (string)null);
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.ChangePasswordActivity", b =>
                {
                    b.HasBaseType("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.Activity");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Type");

                    b.HasDiscriminator().HasValue("ChangePasswordActivity");
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.ChangeUserOtpStatusActivity", b =>
                {
                    b.HasBaseType("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.Activity");

                    b.Property<bool>("IsEnabled")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("bit")
                        .HasColumnName("IsActive");

                    b.HasDiscriminator().HasValue("ChangeUserOtpStatusActivity");
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.ChangeUserRolesActivity", b =>
                {
                    b.HasBaseType("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.Activity");

                    b.Property<string>("Role")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Role");

                    b.HasDiscriminator().HasValue("ChangeUserRolesActivity");
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.ChangeUserStatusActivity", b =>
                {
                    b.HasBaseType("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.Activity");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("bit")
                        .HasColumnName("IsActive");

                    b.HasDiscriminator().HasValue("ChangeUserStatusActivity");
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.ForgetPasswordActivity", b =>
                {
                    b.HasBaseType("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.Activity");

                    b.HasDiscriminator().HasValue("ForgetPasswordActivity");
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.RegisterUserActivity", b =>
                {
                    b.HasBaseType("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.Activity");

                    b.Property<bool>("ByAdmin")
                        .HasColumnType("bit")
                        .HasColumnName("ByAdmin");

                    b.Property<string>("Email")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Email");

                    b.Property<string>("Name")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.Property<string>("PersonType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("PhoneNumber");

                    b.HasDiscriminator().HasValue("RegisterUserActivity");
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.ResendCodeActivity", b =>
                {
                    b.HasBaseType("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.Activity");

                    b.Property<string>("CodeType")
                        .IsRequired()
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("CodeType");

                    b.Property<string>("PhoneNumber")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("PhoneNumber");

                    b.HasDiscriminator().HasValue("ResendCodeActivity");
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.SignInActivity", b =>
                {
                    b.HasBaseType("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.Activity");

                    b.Property<string>("ResultType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("ResultType");

                    b.Property<string>("SignInType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("SignInType");

                    b.HasDiscriminator().HasValue("SignInActivity");
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.UpdateProfileActivity", b =>
                {
                    b.HasBaseType("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.Activity");

                    b.Property<string>("Email")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Email");

                    b.Property<string>("Name")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.Property<string>("PhoneNumber")
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("PhoneNumber");

                    b.Property<bool?>("TwoFactorEnabled")
                        .HasColumnType("bit")
                        .HasColumnName("TwoFactorEnabled");

                    b.HasDiscriminator().HasValue("UpdateProfileActivity");
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.VerifyCodeActivity", b =>
                {
                    b.HasBaseType("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.Activity");

                    b.Property<string>("CodeType")
                        .IsRequired()
                        .ValueGeneratedOnUpdateSometimes()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("CodeType");

                    b.HasDiscriminator().HasValue("VerifyCodeActivity");
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.Groups.Entities.GroupPermission", b =>
                {
                    b.HasOne("Dgab.Authentication.DomainModel.Modules.Groups.Group", "Group")
                        .WithMany("GroupPermissions")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Dgab.Authentication.DomainModel.Modules.Permissions.Permission", "Permission")
                        .WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Group");

                    b.Navigation("Permission");
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.Groups.Entities.GroupUser", b =>
                {
                    b.HasOne("Dgab.Authentication.DomainModel.Modules.Groups.Group", "Group")
                        .WithMany("GroupUsers")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Group");
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.Activity", b =>
                {
                    b.HasOne("Dgab.Authentication.DomainModel.Modules.UserActivities.UserActivity", null)
                        .WithOne("Activity")
                        .HasForeignKey("Dgab.Authentication.DomainModel.Modules.UserActivities.Activities.Activity", "UserActivityId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.UserActivities.UserActivity", b =>
                {
                    b.OwnsOne("Dgab.Authentication.DomainModel.Modules.UserActivities.ValueObjects.ClientInfo", "ClientInfo", b1 =>
                        {
                            b1.Property<Guid>("UserActivityId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Agent")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("Device")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("IP")
                                .HasColumnType("nvarchar(max)");

                            b1.Property<string>("OS")
                                .HasColumnType("nvarchar(max)");

                            b1.HasKey("UserActivityId");

                            b1.ToTable("UserActivities");

                            b1.WithOwner()
                                .HasForeignKey("UserActivityId");
                        });

                    b.Navigation("ClientInfo");
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.Groups.Group", b =>
                {
                    b.Navigation("GroupPermissions");

                    b.Navigation("GroupUsers");
                });

            modelBuilder.Entity("Dgab.Authentication.DomainModel.Modules.UserActivities.UserActivity", b =>
                {
                    b.Navigation("Activity");
                });
#pragma warning restore 612, 618
        }
    }
}
