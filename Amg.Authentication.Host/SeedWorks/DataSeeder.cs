using System;
using System.Linq;
using System.Threading.Tasks;
using Amg.Authentication.DomainModel.Modules.Groups;
using Amg.Authentication.DomainModel.Modules.Groups.Entities;
using Amg.Authentication.DomainModel.Modules.Permissions;
using Amg.Authentication.DomainModel.Modules.Roles;
using Amg.Authentication.DomainModel.Modules.Users;
using Amg.Authentication.Host.Services;
using Amg.Authentication.Infrastructure.Enums;
using Amg.Authentication.Infrastructure.Extensions;
using Amg.Authentication.Persistence.Contexts;
using Amg.Authentication.Shared.Permissions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Amg.Authentication.Host.SeedWorks
{
    public static class DataSeeder
    {
        public static void/*IHost*/ Seed(this WebApplication builder)
        {
            try
            {
                using var scope = builder.Services.CreateScope();
                var serviceProvider = scope.ServiceProvider;

                // update database context
                var databaseContext = serviceProvider.GetRequiredService<DatabaseContext>();
                databaseContext.Database.Migrate();
                CreatePermissionsIfNotExists(databaseContext).Wait();

                // update identity context
                var identityContext = serviceProvider.GetRequiredService<IdentityContext>();
                var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
                identityContext.Database.Migrate();
                CreateRolesIfNotExists(roleManager).Wait();
                CreateSuperAdminIfNotExists(userManager).Wait();
                CreateAdminIfNotExists(userManager, databaseContext).Wait();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            //return host;
        }

        private static async Task CreatePermissionsIfNotExists(DatabaseContext context)
        {
            var systemPermissions = PermissionsScanner.GetAllPermissions(typeof(AuthenticationPermissions).Assembly);
            var permissionsDbSet = context.Set<Permission>();
            var groupPermissionsDbSet = context.Set<GroupPermission>();
            var databasePermissions = await permissionsDbSet.AsQueryable().ToListAsync();

            // add new permissions
            foreach (var sp in systemPermissions
                .Where(sp => databasePermissions.All(dp => sp.Name != dp.Name)))
            {
                permissionsDbSet.Add(new Permission(sp.Name, sp.Description, sp.ServiceName, sp.Category));
                WriteColor($"Added New Permission: {sp.Name}.", ConsoleColor.Yellow);
            }
            await context.SaveChangesAsync();

            foreach (var dp in databasePermissions
                .Where(dp => systemPermissions.All(sp => dp.Name != sp.Name)))
            {
                var groupPermissionsToDelete = await groupPermissionsDbSet.AsQueryable()
                    .Where(i => i.Permission.Name == dp.Name).ToListAsync();
                groupPermissionsDbSet.RemoveRange(groupPermissionsToDelete);
                permissionsDbSet.Remove(dp);
                WriteColor($"Deleted Unused Permission {dp.Name} with {groupPermissionsToDelete.Count} GroupPermissions.",
                    ConsoleColor.DarkYellow);
            }
            await context.SaveChangesAsync();

        }

        #region PrivateMethods

        private static async Task CreateRolesIfNotExists(RoleManager<Role> roleManager)
        {
            var systemRoles = EnumExtensions.GetValues<RoleType>();
            foreach (var role in systemRoles)
            {
                var currentRole = await roleManager.FindByNameAsync(role.ToString());
                if (currentRole == null)
                    await roleManager.CreateAsync(new Role(role.ToString()));
            }
        }

        private static async Task CreateSuperAdminIfNotExists(UserManager<User> userManager)
        {
            const string username = "Administrator";
            const string password = "P@ssw0rd";
            const string firstname = "کاربر";
            const string lastname = "ارشد";

            var currentAdmin = await userManager.FindByNameAsync(username);
            if (currentAdmin == null)
            {
                var admin = new User(username, firstname, lastname, PersonType.Individual, null, null, null)
                {
                    Id = Guid.NewGuid(),
                    UserName = username,
                    PhoneNumberConfirmed = true
                };
                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, RoleType.SuperAdmin.ToString());
            }
        }


        private static async Task CreateAdminIfNotExists(UserManager<User> userManager, DatabaseContext context)
        {
            const string username = "Admin";
            const string password = "P@ssw0rd";
            const string firstname = "مدیر";
            const string lastname = "پنل";

            var systemPermissions = PermissionsScanner.GetAllPermissions(typeof(AuthenticationPermissions).Assembly);
            var permissionsDbSet = context.Set<Permission>();
            var groupPermissionsDbSet = context.Set<GroupPermission>();
            var groupDbSet = context.Set<Group>();
            var groupUserDbSet = context.Set<GroupUser>();
            var databasePermissions = await permissionsDbSet.AsQueryable().ToListAsync();

            var currentAdmin = await userManager.FindByNameAsync(username);
            if (currentAdmin is not null)
                return;

            var admin = new User(username, firstname, lastname, PersonType.Individual, null, null, null)
            {
                Id = Guid.NewGuid(),
                UserName = username,
                PhoneNumberConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, password);
            if (!result.Succeeded)
                return;

            await userManager.AddToRoleAsync(admin, RoleType.SystemUser.ToString());

            var group = await groupDbSet.FirstOrDefaultAsync(x => x.Name == "Admin");
            var adminGroup = new Group("Admin");
            if (group is null)
                groupDbSet.Add(adminGroup);

            var adminGroups = groupUserDbSet.Where(x => x.UserId == admin.Id).ToList();
            if (!adminGroups.Any() && !adminGroups.Any(x => x.Group.Name == "Admin"))
                groupUserDbSet.Add(new GroupUser(adminGroup, admin.Id));

            var adminPermissions = groupPermissionsDbSet.Where(x => x.Group.Id == adminGroup.Id).ToList();

            foreach (var sp in systemPermissions.Where(sp => adminPermissions.All(dp => sp.Name != dp.Permission.Name)))
            {
                permissionsDbSet.Add(new Permission(sp.Name, sp.Description, sp.ServiceName, sp.Category));
                WriteColor($"Added New Permission: {sp.Name} To Admin.", ConsoleColor.Yellow);
            }
            await context.SaveChangesAsync();

        }

        private static void WriteColor(object value, ConsoleColor color)
        {
            var cl = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ForegroundColor = cl;
        }

        #endregion
    }

}
