// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataAuthWebApp.Data;
using DataLayer.EfClasses.MultiTenantClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using PermissionParts;

namespace DataAuthWebApp.DisplayCode
{

    public class ListUsers
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly MultiTenantDbContext _multiTenantDbContext;

        public ListUsers(ApplicationDbContext applicationDbContext, MultiTenantDbContext multiTenantDbContext)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _multiTenantDbContext = multiTenantDbContext ?? throw new ArgumentNullException(nameof(multiTenantDbContext));
        }

        public List<UserListDto> ListUserWithRolesAndModules()
        {
            var rolesWithUserIds = _applicationDbContext.UserRoles
                .Select(x => new { _applicationDbContext.Roles.Single(y => y.Id == x.RoleId).Name, x.UserId }).ToList();

            var result = new List<UserListDto>();
            foreach (var user in _applicationDbContext.Users)
            {
                var mUser = _multiTenantDbContext.MultiTenantUsers.IgnoreQueryFilters()
                    .Include(x => x.AccessTo)
                    .SingleOrDefault(x => x.UserId == user.Id);
                if (mUser != null)
                {
                    var shopNamesLinked = mUser.IsDistrictManager
                        ? string.Join(", ", mUser.AccessTo.Select(x => x.Name))
                        : _multiTenantDbContext.Shops.IgnoreQueryFilters().Single(x => x.ShopKey == mUser.ShopKey).Name;
                    result.Add(new UserListDto(user.UserName,
                        string.Join(", ", rolesWithUserIds.Where(x => x.UserId == user.Id).Select(x => x.Name)),
                        mUser.IsDistrictManager, shopNamesLinked));
                }
                else
                {
                    result.Add(new UserListDto(user.UserName,
                        string.Join(", ", rolesWithUserIds.Where(x => x.UserId == user.Id).Select(x => x.Name)),
                        false, "-- Unassigned --"));
                }
            }

            return result;
        }
    }
}