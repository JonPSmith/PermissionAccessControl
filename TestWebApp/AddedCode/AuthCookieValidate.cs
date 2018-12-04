// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using PermissionParts;
using TestWebApp.Data;

namespace TestWebApp.AddedCode
{
    public class AuthCookieValidate
    {
        private DbContextOptions<RolesDbContext> _rolesDbOptions;

        public AuthCookieValidate(DbContextOptions<RolesDbContext> rolesDbOptions)
        {
            _rolesDbOptions = rolesDbOptions;
        }

        public Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            if (context.Principal.Claims.Any(x => x.Type == PermissionConstants.PackedPermissionClaimType))
                return Task.CompletedTask;

            //No permissions in the claims, so add it
            var claims = new List<Claim>();
            foreach (var claim in context.Principal.Claims)
            {
                claims.Add(claim);
            }

            var usersRoles = context.Principal.Claims.Where(x => x.Type == ClaimTypes.Role).Select(x => x.Value)
                .ToList();
            using (var dbContext = new RolesDbContext(_rolesDbOptions))
            {
                var permissionsForUser = dbContext.RolesToPermissions.Where(x => usersRoles.Contains(x.RoleName))
                    .SelectMany(x => x.PermissionsInRole)
                    .Distinct()
                    .ToList();
                //Now add it to the claim
                claims.Add(new Claim(PermissionConstants.PackedPermissionClaimType,
                    permissionsForUser.PackPermissionsIntoString()));
            }

            var identity = new ClaimsIdentity(claims, "Cookie");
            var newPrincipal = new ClaimsPrincipal(identity);

            context.ReplacePrincipal(newPrincipal);
            context.ShouldRenew = true;

            return Task.CompletedTask;
        }
    }
}