// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace PermissionParts
{
    public class PermissionDisplay
    {
        public PermissionDisplay(int value, string name, string description)
        {
            Value = value;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }

        public int Value { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }

        /// <summary>
        /// This builds the list of the permissions, with their grouping, to show when building Roles
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, List<PermissionDisplay>> GetPermissionsGrouped(Type enumType) 
        {
            var resultDict = new Dictionary<string, List<PermissionDisplay>>();
            foreach (var permissionName in Enum.GetNames(enumType))
            {
                var member = enumType.GetMember(permissionName);
                //This allows you to obsolete a permission and it won't be shown as a possible option, but is still there so you won't reuse the number
                var obsoleteAttribute = member[0].GetCustomAttribute<ObsoleteAttribute>();
                if (obsoleteAttribute != null)
                    continue;
                var displayAttribute = member[0].GetCustomAttribute<DisplayAttribute>();
                if (displayAttribute == null)
                    continue;

                if (!resultDict.ContainsKey(displayAttribute.GroupName))
                    resultDict[displayAttribute.GroupName] = new List<PermissionDisplay>();
                var permission = Enum.Parse(enumType, permissionName, false);
                resultDict[displayAttribute.GroupName].Add(new PermissionDisplay((int)permission, permissionName, displayAttribute.Description));
            }

            return resultDict;
        }
    }
}