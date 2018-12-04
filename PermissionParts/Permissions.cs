// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.ComponentModel.DataAnnotations;

namespace PermissionParts
{
    public enum Permissions
    {
        NotSet = 0, //error condition

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
        [Display(GroupName = "UserAdmin", Name = "Create user", Description = "can create a new User")]
        UserCreate = 0x21,
        [Display(GroupName = "UserAdmin", Name = "Update user", Description = "can update a User")]
        UserUpdate = 0x22,
        [Display(GroupName = "UserAdmin", Name = "Delete user", Description = "can delete a User")]
        UserDelete = 0x23,

        [Display(GroupName = "UserAdmin", Name = "Read Role", Description = "can list Role")]
        RoleRead = 0x28,
        [Display(GroupName = "UserAdmin", Name = "Create Role", Description = "can create a new Role")]
        RoleCreate = 0x29,
        [Display(GroupName = "UserAdmin", Name = "Update Role", Description = "can update a Role")]
        RoleUpdate = 0x2A,
        [Display(GroupName = "UserAdmin", Name = "Delete Role", Description = "can delete a Role")]
        RoleDelete = 0x2B,
    }
}