// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;

namespace PermissionParts
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PermissionLinkedToModuleAttribute : Attribute
    {
        public Modules Module { get; private set; }

        public PermissionLinkedToModuleAttribute(Modules module)
        {
            Module = module;
        }
    }
}