// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using DataLayer.EfClasses;
using DataLayer.EfClasses.AuthClasses;
using DataLayer.EfClasses.MultiTenantClasses;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using PermissionParts;

namespace StartupCode
{
    public static class StartupExtensions
    {
        private const string SeedDataDir = "SeedData";
        private const string UserInfoJsonFilename = "userinfo.json";
        private const string RoleToPermissionsFilename = "rolestopermissions.json";

        public static async Task AddUsersAndExtraAuthAsync(this IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var env = services.GetRequiredService<IHostingEnvironment>();
                var pathToUserData = Path.GetFullPath(Path.Combine(env.WebRootPath, SeedDataDir, UserInfoJsonFilename));
                var userInfos = JsonConvert.DeserializeObject<List<UserInfoJson>>(File.ReadAllText(pathToUserData));

                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                var users = new List<IdentityUser>();
                foreach (var userInfoJson in userInfos)
                {
                    users.Add(await AddUserWithRoles(userInfoJson, userManager, roleManager));
                }

                var pathToRolePermissionData = Path.GetFullPath(Path.Combine(env.WebRootPath, SeedDataDir, RoleToPermissionsFilename));
                var rolesToPermissions = JsonConvert.DeserializeObject<List<RoleToPermissions>>(File.ReadAllText(pathToRolePermissionData));

                using (var context = services.GetRequiredService<ExtraAuthorizeDbContext>())
                {
                    context.AddRange(rolesToPermissions);
                    context.AddRange(userInfos.BuildModulesForUsers(users));
                    context.SaveChanges();
                }

                if (userInfos.Any(x => x.ShopNames != null))
                    using (var context = services.GetRequiredService<MultiTenantDbContext>())
                    {
                        context.Database.EnsureCreated();

                        var shops = context.SetupMultiTenantUsers(userInfos, users);
                        context.SaveChanges();
                        context.SetupStockInShops(shops);
                        context.SaveChanges();
                    }
            }
        }

        //---------------------------------------------------------------------------
        //private methods

        private static IEnumerable<Shop> SetupMultiTenantUsers(this MultiTenantDbContext context, List<UserInfoJson> userInfos,
            List<IdentityUser> users)
        {
            var shopsDict = new Dictionary<string,Shop>();

            IEnumerable<Shop> AddOrFindShops(string[] shopNames)
            {
                foreach (var shopName in shopNames)
                {
                    if (!shopsDict.ContainsKey(shopName))
                    {
                        var shop = new Shop { Name = shopName };
                        shop.SetShopKey(shopsDict.Values.Count + 1);  //set the shopKey, as used later
                        shopsDict[shopName] = shop;
                    }

                    yield return shopsDict[shopName];
                }
            }

            foreach (var userInfo in userInfos)
            {
                if (userInfo.ShopNames != null)
                {
                    var shops = AddOrFindShops(userInfo.ShopNames.Split(',')).ToList();
                    var mUser = shops.Count == 1
                        ? new MultiTenantUser
                            {UserId = users.Single(x => x.Email == userInfo.Email).Id, ShopKey = shops.Single().ShopKey }
                        : new MultiTenantUser
                        {
                            UserId = users.Single(x => x.Email == userInfo.Email).Id,
                            //ShopKey is not set, i.e. it defaults to zero
                            IsDistrictManager = true,
                            AccessTo = shops
                        };
                    context.Add(mUser);
                }
            }
            return shopsDict.Values;
        }

        private static void SetupStockInShops(this MultiTenantDbContext context, IEnumerable<Shop> shops)
        {
            foreach (var shop in shops)
            {
                var stock1 = new StockInfo {Name = $"{shop.Name} nice stuff", NumInStock = 10, AtShop = shop, DistrictManagerId = shop.DistrictManagerId};
                var stock2 = new StockInfo { Name = $"{shop.Name} other stuff", NumInStock = 22, AtShop = shop, DistrictManagerId = shop.DistrictManagerId };
                var stock3 = new StockInfo { Name = $"{shop.Name} thingy", NumInStock = 3, AtShop = shop, DistrictManagerId = shop.DistrictManagerId };
                context.AddRange(stock1, stock2, stock3);
            }
        }

        private static IEnumerable<ModulesForUser> BuildModulesForUsers(this List<UserInfoJson> userInfo,
            List<IdentityUser> users)
        {
            foreach (var userInfoJson in userInfo.Where(x => !string.IsNullOrEmpty(x.ModulesCommaDelimited)))
            {
                PaidForModules combinedModules = PaidForModules.None;
                foreach (var moduleName in userInfoJson.ModulesCommaDelimited.Split(',').Select(x => x.Trim()))
                {
                    if (!Enum.TryParse(typeof(PaidForModules), moduleName, true, out var thisModule))
                        throw new InvalidOperationException($"The module name {moduleName} in the {UserInfoJsonFilename} isn't a valid module name.");
                    combinedModules |= (PaidForModules) thisModule;
                }
                yield return new ModulesForUser(userInfoJson.Email.GetUserIdWithGivenEmail(users), combinedModules);
            }
        }

        private static string GetUserIdWithGivenEmail(this string email, List<IdentityUser> users)
        {
            return users.Single(x => x.Email == email).Id;
        }

        private static async Task<IdentityUser> AddUserWithRoles(UserInfoJson userInfo, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var user = new IdentityUser {UserName = userInfo.Email, Email = userInfo.Email};
            var result = await userManager.CreateAsync(user, user.Email); //email is the password
            if (!result.Succeeded)
                throw new InvalidOperationException($"Tried to add user {user.UserName}, but failed.");

            foreach (var roleName in userInfo.RolesCommaDelimited.Split(',').Select(x => x.Trim()))
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
                await userManager.AddToRoleAsync(user, roleName);
            }

            return user;
        }
    }
}