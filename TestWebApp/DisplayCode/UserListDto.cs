// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace TestWebApp.DisplayCode
{
    public class UserListDto
    {
        public UserListDto(string email, string roleNames, string moduleNames)
        {
            Email = email ?? throw new ArgumentNullException(nameof(email));
            RoleNames = roleNames ?? throw new ArgumentNullException(nameof(roleNames));
            ModuleNames = moduleNames ?? throw new ArgumentNullException(nameof(moduleNames));
        }

        public string Email { get; set; }
        public string RoleNames { get; set; }
        public string ModuleNames { get; set; }


    }
}