// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace DataAuthWebApp.DisplayCode
{
    public class UserListDto
    {
        public UserListDto(string email, string roleNames, string shopName)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            RoleNames = roleNames ?? throw new ArgumentNullException(nameof(roleNames));
            ShopName = shopName ?? throw new ArgumentNullException(nameof(shopName));
        }

        public string Email { get; set; }
        public string RoleNames { get; set; }
        public string ShopName { get; set; }


    }
}