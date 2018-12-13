using System;
using System.Collections.Generic;
using System.Linq;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Mvc;
using PermissionParts;
using TestWebApp.Data;
using TestWebApp.DisplayCode;

namespace TestWebApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ExtraAuthorizeDbContext _extraAuthorizeDbContext;

        public UsersController(ApplicationDbContext applicationDbContext, ExtraAuthorizeDbContext extraAuthorizeDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _extraAuthorizeDbContext = extraAuthorizeDbContext;
        }

        public IActionResult Index()
        {
            return View(HttpContext.User);
        }

        public IActionResult List()
        {
            var lister = new ListUsers(_applicationDbContext, _extraAuthorizeDbContext);

            return View(lister.ListUserWithRolesAndModules());
        }


        public IActionResult Roles()
        {
            var roles = _extraAuthorizeDbContext.RolesToPermissions.ToList();
            return View(roles);
        }

        public IActionResult Permissions()
        {
            return View(PermissionDisplay.GetPermissionsToDisplay(typeof(Permissions)));
        }
    }
}
