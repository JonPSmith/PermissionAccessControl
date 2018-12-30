// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;

namespace DataAuthWebApp.DisplayCode
{
    public class HomePageDto
    {
        public HomePageDto(List<UserListDto> user, List<RolesListDto> roles)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            Roles = roles ?? throw new ArgumentNullException(nameof(roles));
        }

        public List<UserListDto> User { get; set; }
        public List<RolesListDto> Roles { get; set; }
    }
}