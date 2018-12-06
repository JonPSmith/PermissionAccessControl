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
        private const string DataReadOnlyRole = "DataReadOnly";
        private const string DataReadWriteRole = "DataReadWrite";
        private const string UserAdminRole = "UserAdmin";
        private static readonly List<RoleToPermissions> DefaultRoles = new List<RoleToPermissions>
        {
            new RoleToPermissions(DataReadOnlyRole, "User can read the data, but nothing else", new List<Permissions>{ Permissions.DataRead}),
            new RoleToPermissions(DataReadWriteRole, "User can create, read, update or delete data", 
                new List<Permissions>{ Permissions.DataRead, Permissions.DataCreate, Permissions.DataDelete, Permissions.DataUpdate}),
            new RoleToPermissions(UserAdminRole, "This user can do anything with Roles and User",
                new List<Permissions>
                {
                    Permissions.UserRead, Permissions.RoleRead, Permissions.RoleChange, Permissions.UserChange, 
                }),
        };

        //NOTE: Name must be an email
        private static readonly List<IdentityUser> DefaultUsers = new List<IdentityUser>
        {
            new IdentityUser{ UserName = "UR1@gmail.com", Email = "UR1@gmail.com"},
            new IdentityUser{ UserName = "UW1@gmail.com", Email = "UW1@gmail.com"},
            new IdentityUser{ UserName = "Admin1@gmail.com", Email = "Admin1@gmail.com"},
        };


        public static void SetupDatabases(this IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                using (var context = services.GetRequiredService<RolesDbContext>())
                {
                    context.Database.EnsureCreated();
                    context.AddRange(DefaultRoles);
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
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
                var addRoleResult = await userManager.AddToRoleAsync(user, roleName);
            }
        }
    }
}