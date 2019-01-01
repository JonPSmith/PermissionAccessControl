// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace DataAuthWebApp.DisplayCode
{
    public class UserListDto
    {
        public UserListDto(string email, string roleNames, bool isDistrictManager, string shopNames)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            RoleNames = roleNames ?? throw new ArgumentNullException(nameof(roleNames));
            IsDistrictManager = isDistrictManager;
            ShopNames = shopNames ?? throw new ArgumentNullException(nameof(shopNames));
        }

        public string Email { get; set; }
        public string RoleNames { get; set; }
        public bool IsDistrictManager { get; set; }
        public string ShopNames { get; set; }


    }
}