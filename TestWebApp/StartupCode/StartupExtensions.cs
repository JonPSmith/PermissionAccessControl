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

namespace TestWebApp.StartupCode
{
    public static class StartupExtensions
    {
        private static List<RoleToPermissions> _defaultRoles = new List<RoleToPermissions>
        {
            new RoleToPermissions("DataReadOnly", "User can read the data, but nothing else", new List<Permissions>{ Permissions.DataRead}),
            new RoleToPermissions("DataReadWrite", "User can create, read, update or delete data", 
                new List<Permissions>{ Permissions.DataRead, Permissions.DataCreate, Permissions.DataDelete, Permissions.DataUpdate}),
            new RoleToPermissions("UserAdmin", "This user can do anything with Roles and User",
                new List<Permissions>
                {
                    Permissions.RoleCreate, Permissions.RoleDelete, Permissions.RoleRead, Permissions.RoleUpdate,
                    Permissions.UserCreate, Permissions.UserDelete, Permissions.UserRead, Permissions.UserUpdate
                }),
        };

        private static List<IdentityUser> _defaultUsers = new List<IdentityUser>
        {
            new IdentityUser{ UserName = "UserRead", Email = "UR1@gmail.com"},
            new IdentityUser{ UserName = "UserWrite", Email = "UW1@gmail.com"},
            new IdentityUser{ UserName = "Admin", Email = "Admin1@gmail.com"},
        };


        public static void SetupDatabases(this IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                using (var context = services.GetRequiredService<RolesDbContext>())
                {
                    context.Database.EnsureCreated();
                    context.AddRange(_defaultRoles);
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

                await AddUserWithRoles(_defaultUsers[0], userManager, roleManager, "DataReadOnly");
                await AddUserWithRoles(_defaultUsers[1], userManager, roleManager, "DataReadWrite");
                await AddUserWithRoles(_defaultUsers[2], userManager, roleManager, "DataReadWrite", "UserAdmin");
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