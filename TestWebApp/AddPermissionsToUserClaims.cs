// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Security.Claims;
using System.Threading.Tasks;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RolesToPermission;

namespace TestWebApp
{
    //thanks to https://korzh.com/blogs/net-tricks/aspnet-identity-store-user-data-in-claims
    public class AddPermissionsToUserClaims : UserClaimsPrincipalFactory<IdentityUser, IdentityRole>
    {
        private readonly ExtraAuthorizeDbContext _extraAuthDbContext;

        public AddPermissionsToUserClaims(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor, 
            ExtraAuthorizeDbContext extraAuthDbContext)
            : base(userManager, roleManager, optionsAccessor)
        {
            _extraAuthDbContext = extraAuthDbContext;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(IdentityUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            var rtoPCalcer = new CalcAllowedPermissions(_extraAuthDbContext);
            identity.AddClaim(new Claim(PermissionConstants.PackedPermissionClaimType, 
                await rtoPCalcer.CalcPermissionsForUser(identity.Claims)));
            return identity;
        }
    }
}