// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.AspNetCore.Mvc;
using PermissionParts;
using TestWebApp.RolesToPermissions;

namespace TestWebApp.Controllers
{
    public class Feature1Controller : Controller
    {
        [HasPermission(Permissions.Feature1Access)]
        public IActionResult Index()
        {
            return
            View();
        }
    }
}