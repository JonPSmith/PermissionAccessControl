// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DataAuthorize;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RolesToPermission;

namespace DataAuthWebApp
{
    public class DataCookieValidate
    {
        private readonly DbContextOptions<MultiTenantDbContext> _multiTenantOptions;

        public DataCookieValidate(DbContextOptions<MultiTenantDbContext> multiTenantOptions)
        {
            _multiTenantOptions = multiTenantOptions;
        }

        public async Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            //NOTE: To make easier to see the data authorization code I have removed  
            //all the feature authorization code described in the article
            //https://www.thereformedprogrammer.net/a-better-way-to-handle-authorization-in-asp-net-core/
            //BUT in real life this method with have both the feature authorization and data authorization code in it


            if (context.Principal.Claims.Any(x => x.Type == GetClaimsFromUser.ShopKeyClaimName))
                return;

            //No ShopKey in the claims, so we need to add it. This is only happens once after the user has logged in
            var claims = new List<Claim>();
            claims.AddRange(context.Principal.Claims); //Copy over existing claims

            //now we lookup the user to find what shop they are linked to
            using (var multiContext = new MultiTenantDbContext(_multiTenantOptions, new DummyClaimsFromUser()))
            {
                var userId = context.Principal.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var mTUser = await multiContext.MultiTenantUsers.IgnoreQueryFilters()
                    .SingleOrDefaultAsync(x => x.UserId == userId);
                //This means unassigned users are given a ShopKey of zero, which means unassigned
                var shopKey = mTUser?.ShopKey ?? 0;                
                claims.Add(new Claim(GetClaimsFromUser.ShopKeyClaimName, shopKey.ToString()));
                if (mTUser?.IsDistrictManager == true)
                    claims.Add(new Claim(GetClaimsFromUser.DistrictManagerIdClaimName, mTUser.UserId));
            }

            //Build a new ClaimsPrincipal and use it to replace the current ClaimsPrincipal
            var identity = new ClaimsIdentity(claims, "Cookie");
            var newPrincipal = new ClaimsPrincipal(identity);
            context.ReplacePrincipal(newPrincipal);
            //THIS IS IMPORTANT: This updates the cookie, otherwise this calc will be done every HTTP request
            context.ShouldRenew = true;  
        }
    }
}