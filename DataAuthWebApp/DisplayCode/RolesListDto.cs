// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace DataAuthWebApp.DisplayCode
{
    public class RolesListDto
    {
        public RolesListDto(string roleName, List<PermissionWithDesc> permissionsWithDesc)
        {
            RoleName = roleName ?? throw new ArgumentNullException(nameof(roleName));
            PermissionsWithDesc = permissionsWithDesc ?? throw new ArgumentNullException(nameof(permissionsWithDesc));
        }

        public string RoleName { get; set; }
        public List<PermissionWithDesc> PermissionsWithDesc { get; set; }
    }

    public class PermissionWithDesc
    {
        public PermissionWithDesc(string permissionName, string description, string linkedToModule)
        {
            PermissionName = permissionName ?? throw new ArgumentNullException(nameof(permissionName));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            LinkedToModule = linkedToModule;
        }

        public string PermissionName { get; set; }
        public string Description { get; set; }
        public string LinkedToModule { get; set; }
    }
}