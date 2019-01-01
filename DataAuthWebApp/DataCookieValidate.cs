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
            if (context.Principal.Claims.Any(x => x.Type == GetClaimsFromUser.ShopKeyClaimName))
                return;

            //No ShopKey in the claims, so we need to add it. This is only happens once after the user has logged in
            var claims = new List<Claim>();
            claims.AddRange(context.Principal.Claims); //Copy over existing claims

            //now we lookup the user to find what shop they are linked to
            using (var multiContext = new MultiTenantDbContext(_multiTenantOptions, new DummyClaimsFromUser()))
            {
                var userId = context.Principal.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value;
                var mTUser = multiContext.MultiTenantUsers.IgnoreQueryFilters().SingleOrDefault(x => x.UserId == userId);
                if (mTUser == null)
                    throw new InvalidOperationException($"The user {context.Principal.Claims.Single(x => x.Type == ClaimTypes.Name).Value} was not linked to a shop.");
                claims.Add(new Claim(GetClaimsFromUser.ShopKeyClaimName, mTUser.ShopKey.ToString()));
                if (mTUser.IsDistrictManager)
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