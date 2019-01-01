// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace StartupCode
{
    public class UserInfoJson
    {
        /// <summary>
        /// User's Email, which is also their password
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// List of RoleNames, comma delimited
        /// </summary>
        public string RolesCommaDelimited { get; set; }
        /// <summary>
        /// List of Module Names, comma delimited
        /// </summary>
        public string ModulesCommaDelimited { get; set; }
        /// <summary>
        /// Various Data authorization data
        /// </summary>
        public string ShopNames { get; set; }
    }
}