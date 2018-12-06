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
        [Display(GroupName = "Data", Name = "Read", Description = "can read data")]
        DataRead = 0x10,
        [Display(GroupName = "Data", Name = "Create", Description = "can create data")]
        DataCreate = 0x11,
        [Display(GroupName = "Data", Name = "Update", Description = "can update data")]
        DataUpdate = 0x12,
        [Display(GroupName = "Data", Name = "Delete", Description = "can delete data")]
        DataDelete = 0x13,

        [Display(GroupName = "UserAdmin", Name = "Read user", Description = "can list User")]
        UserRead = 0x20,
        //This is an example of grouping multiple actions under one permission
        [Display(GroupName = "UserAdmin", Name = "Alter user", Description = "can do anything to the User")]
        UserChange = 0x21,

        [Display(GroupName = "UserAdmin", Name = "Read Role", Description = "can list Role")]
        RoleRead = 0x28,
        [Display(GroupName = "UserAdmin", Name = "Change Role", Description = "can create, update or delete a Role")]
        RoleChange = 0x29,

        //This is an example of a permission that is linked to a option (paid for?) feature
        //The code that turns roles to permissions can remove this permission if the user isn't allowed to access this feature
        [PermissionLinkedToModule(Modules.Feature1)]
        [Display(GroupName = "Feature1", Name = "Access", Description = "can allows a user to access feature1")]
        Feature1Access = 0x31,

        //This is an example of what to do with permission you don't used anymore.
        //You don't want its number to be reused as it could cause problems 
        //Just mark it as obsolete and the PermissionDisplay code won't show it
        [Obsolete]
        [Display(GroupName = "Old", Name = "Not used", Description = "example of old permission")]
        OldPermissionNotUsed = 0x40,
    }
}