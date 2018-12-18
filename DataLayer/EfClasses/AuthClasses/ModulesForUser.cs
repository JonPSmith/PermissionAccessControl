// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.ComponentModel.DataAnnotations;
using PermissionParts;

namespace DataLayer.EfClasses.AuthClasses
{
    /// <summary>
    /// This holds what modules a user can access, using the user's identity key
    /// </summary>
    public class ModulesForUser
    {
        public ModulesForUser(string userId, PaidForModules allowedPaidForModules)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            AllowedPaidForModules = allowedPaidForModules;
        }

        [Key]
        [MaxLength(450)] //Matches identity size
        public string UserId { get; set; }
        public PaidForModules AllowedPaidForModules { get; set; }
    }
}