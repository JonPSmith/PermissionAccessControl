// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfCode;
using PermissionParts;
using TestWebApp.Data;

namespace TestWebApp.DisplayCode
{

    public class ListUsers
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ExtraAuthorizeDbContext _extraAuthorizeDbContext;

        public ListUsers(ApplicationDbContext applicationDbContext, ExtraAuthorizeDbContext extraAuthorizeDbContext)
        {
            _applicationDbContext = applicationDbContext ?? throw new ArgumentNullException(nameof(applicationDbContext));
            _extraAuthorizeDbContext = extraAuthorizeDbContext ?? throw new ArgumentNullException(nameof(extraAuthorizeDbContext));
        }

        public List<UserListDto> ListUserWithRolesAndModules()
        {
            var rolesWithUserIds = _applicationDbContext.UserRoles
                .Select(x => new { _applicationDbContext.Roles.Single(y => y.Id == x.RoleId).Name, x.UserId }).ToList();

            var result = new List<UserListDto>();
            foreach (var user in _applicationDbContext.Users)
            {
                var thisUserModules = _extraAuthorizeDbContext.ModulesForUsers.Find(user.Id)?.AllowedPaidForModules ??
                                      PaidForModules.None;
                result.Add(new UserListDto(user.UserName,
                    string.Join(", ", rolesWithUserIds.Where(x => x.UserId == user.Id).Select(x => x.Name)),
                    thisUserModules.ToString()
                ));
            }

            return result;
        }
    }
}