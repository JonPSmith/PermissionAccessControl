// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Threading.Tasks;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using PermissionParts;

namespace RolesToPermission
{
    public class CalcAllowedPermissions
    {
        /// <summary>
        /// NOTE: This class is used in OnValidatePrincipal so it can't use DI, so I can't inject the DbContext here because that is dynamic.
        /// Therefore I can pass in the database options because that is a singleton
        /// From that the method can create a valid dbContext to access the database
        /// </summary>
        private readonly DbContextOptions<ExtraAuthorizeDbContext> _extraAuthDbContextOptions;


        public CalcAllowedPermissions(DbContextOptions<ExtraAuthorizeDbContext> extraAuthDbContextOptions)
        {
            _extraAuthDbContextOptions = extraAuthDbContextOptions;
        }

        /// <summary>
        /// This is called if the Permissions that a user needs calculating.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<string> CalcPermissionsForUser(ClaimsPrincipal user)
        {
            var usersRoles = user.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value)
                .ToList();

            using (var dbContext = new ExtraAuthorizeDbContext(_extraAuthDbContextOptions))
            {
                //This gets all the permissions, with a distinct to remove duplicates
                var permissionsForUser = await dbContext.RolesToPermissions.Where(x => usersRoles.Contains(x.RoleName))
                    .SelectMany(x => x.PermissionsInRole)
                    .Distinct()
                    .ToListAsync();
                //we get the modules this user is allowed to see
                var userModules =
                    dbContext.ModulesForUsers.Find(user.Claims.SingleOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value)
                        ?.AllowedPaidForModules ?? PaidForModules.None;
                //Now we remove permissions that are linked to modules that the user has no access to
                var filteredPermissions =
                    from permission in permissionsForUser
                    let moduleAttr = typeof(Permissions).GetMember(permission.ToString())[0]
                        .GetCustomAttribute<LinkedToModuleAttribute>()
                    where moduleAttr == null || userModules.HasFlag(moduleAttr.PaidForModule)
                    select permission;

                return filteredPermissions.PackPermissionsIntoString();
            }
        }
    }
}