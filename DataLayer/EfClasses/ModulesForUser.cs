// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using PermissionParts;

namespace DataLayer.EfClasses
{
    /// <summary>
    /// This holds what modules a user can access, using the user's email as the key
    /// </summary>
    public class ModulesForUser
    {
        public ModulesForUser(string userEmail, PaidForModules allowedPaidForModules)
        {
            UserEmail = userEmail ?? throw new ArgumentNullException(nameof(userEmail));
            AllowedPaidForModules = allowedPaidForModules;
        }

        [Key]
        public string UserEmail { get; set; }
        public PaidForModules AllowedPaidForModules { get; set; }
    }
}