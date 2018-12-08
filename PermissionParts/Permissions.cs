// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;

namespace PermissionParts
{
    public enum Permissions
    {
        NotSet = 0, //error condition

        //Here is an example of very detailed control over something
        [Display(GroupName = "Data", Name = "Read", Description = "Can read data")]
        DataRead = 0x10,
        [Display(GroupName = "Data", Name = "Create", Description = "Can create data")]
        DataCreate = 0x11,
        [Display(GroupName = "Data", Name = "Update", Description = "Can update data")]
        DataUpdate = 0x12,
        [Display(GroupName = "Data", Name = "Delete", Description = "Can delete data")]
        DataDelete = 0x13,

        [Display(GroupName = "UserAdmin", Name = "Read users", Description = "Can list User")]
        UserRead = 0x20,
        //This is an example of grouping multiple actions under one permission
        [Display(GroupName = "UserAdmin", Name = "Alter user", Description = "Can do anything to the User")]
        UserChange = 0x21,

        [Display(GroupName = "UserAdmin", Name = "Read Roles", Description = "Can list Role")]
        RoleRead = 0x28,
        [Display(GroupName = "UserAdmin", Name = "Change Role", Description = "Can create, update or delete a Role")]
        RoleChange = 0x29,

        //This is an example of a permission linked to a optional (paid for?) feature
        //The code that turns roles to permissions can
        //remove this permission if the user isn't allowed to access this feature
        [PermissionLinkedToModule(PaidForModules.Feature1)]
        [Display(GroupName = "Features", Name = "Feature1", Description = "Can access feature1")]
        Feature1Access = 0x30,
        [PermissionLinkedToModule(PaidForModules.Feature2)]
        [Display(GroupName = "Features", Name = "Feature2", Description = "Can access feature2")]
        Feature2Access = 0x31,

        //This is an example of what to do with permission you don't used anymore.
        //You don't want its number to be reused as it could cause problems 
        //Just mark it as obsolete and the PermissionDisplay code won't show it
        [Obsolete]
        [Display(GroupName = "Old", Name = "Not used", Description = "example of old permission")]
        OldPermissionNotUsed = 0x40,
    }
}