// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using DataLayer.EfCode;
using PermissionParts;
using TestWebApp.Data;

namespace TestWebApp.DisplayCode
{
    public class ListRoles
    {
        private readonly ExtraAuthorizeDbContext _extraAuthorizeDbContext;

        public ListRoles(ExtraAuthorizeDbContext extraAuthorizeDbContext)
        {
            _extraAuthorizeDbContext = extraAuthorizeDbContext;
        }

        public IEnumerable<RolesListDto> ListRolesWithPermissionsExplained()
        {
            foreach (var roleToPermissions in _extraAuthorizeDbContext.RolesToPermissions)
            {
                var permissionsWithDesc =
                    from permission in roleToPermissions.PermissionsInRole
                    let displayAttr = typeof(Permissions).GetMember(permission.ToString())[0]
                        .GetCustomAttribute<DisplayAttribute>()
                    let moduleAttr = typeof(Permissions).GetMember(permission.ToString())[0]
                        .GetCustomAttribute<LinkedToModuleAttribute>()
                    select new PermissionWithDesc(permission.ToString(), displayAttr?.Description, moduleAttr?.PaidForModule.ToString());
                yield return new RolesListDto(roleToPermissions.RoleName, permissionsWithDesc.ToList());
            }
        }

    }
}