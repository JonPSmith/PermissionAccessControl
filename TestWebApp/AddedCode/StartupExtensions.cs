// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PermissionParts;
using TestWebApp.Data;

namespace TestWebApp.AddedCode
{
    public static class StartupExtensions
    {
        private const string DataReadOnlyRole = "Staff";
        private const string DataReadWriteRole = "Manager";
        private const string UserAdminRole = "Admin";

        private const string StaffUserEmail = "Staff@g1.com";
        private const string ManagerUserEmail = "Manager@g1.com";
        private const string AdminUserEmail = "Admin@g1.com";

        private static readonly List<RoleToPermissions> DefaultRoles = new List<RoleToPermissions>
        {
            new RoleToPermissions(DataReadOnlyRole, "Staff can only read data", new List<Permissions>{ Permissions.DataRead, Permissions.Feature1Access}),
            new RoleToPermissions(DataReadWriteRole, "Managers can read/write the data", 
                new List<Permissions>{ Permissions.DataRead, Permissions.DataCreate, Permissions.DataDelete, Permissions.DataUpdate, Permissions.Feature1Access}),
            new RoleToPermissions(UserAdminRole, "Admin can manage users, but not read data",
                new List<Permissions> {Permissions.UserRead,Permissions.UserChange, Permissions.Feature1Access, Permissions.Feature2Access }),
        };

        private static readonly List<ModulesForUser> DefaultModules = new List<ModulesForUser>
        {
            new ModulesForUser(StaffUserEmail, PaidForModules.Feature1),
            new ModulesForUser(AdminUserEmail, PaidForModules.Feature1 | PaidForModules.Feature2),
        };

        //NOTE: ShortName must be an email
        private static readonly List<IdentityUser> DefaultUsers = new List<IdentityUser>
        {
            new IdentityUser{ UserName = StaffUserEmail, Email = StaffUserEmail},
            new IdentityUser{ UserName = ManagerUserEmail, Email = ManagerUserEmail},
            new IdentityUser{ UserName = AdminUserEmail, Email = AdminUserEmail},
        };

        public static void SetupDatabases(this IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                using (var context = services.GetRequiredService<ExtraAuthorizeDbContext>())
                {
                    context.Database.EnsureCreated();
                    context.AddRange(DefaultRoles);
                    context.AddRange(DefaultModules);
                    context.SaveChanges();
                }
                using (var context = services.GetRequiredService<ApplicationDbContext>())
                {
                    context.Database.EnsureCreated();
                }
            }
        }

        public static async Task SetupDefaultUsersAsync(this IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await AddUserWithRoles(DefaultUsers[0], userManager, roleManager, DataReadOnlyRole);
                await AddUserWithRoles(DefaultUsers[1], userManager, roleManager, DataReadWriteRole);
                await AddUserWithRoles(DefaultUsers[2], userManager, roleManager, DataReadWriteRole, UserAdminRole);
            }
        }


        //---------------------------------------------------------------------------
        //private methods


        private static async Task AddUserWithRoles(IdentityUser user, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
            params string [] roleNames)
        {
            var result = await userManager.CreateAsync(user, user.Email); //email is the password
            if (!result.Succeeded)
                throw new InvalidOperationException($"Tried to add user {user.UserName}, but failed.");

            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
                await userManager.AddToRoleAsync(user, roleName);
            }
        }
    }
}