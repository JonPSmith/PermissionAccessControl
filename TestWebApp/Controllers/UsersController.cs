using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using PermissionParts;
using TestWebApp.Data;

namespace TestWebApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;

        public UsersController(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IActionResult Index()
        {
            return View(HttpContext.User);
        }

        public IActionResult List()
        {
            var rolesWithUserIds = _applicationDbContext.UserRoles
                .Select(x => new { _applicationDbContext.Roles.Single(y => y.Id == x.RoleId).Name, x.UserId }).ToList();

            var result = new List<Tuple<string, string>>();
            foreach (var user in _applicationDbContext.Users)
            {
                result.Add(new Tuple<string, string>(user.UserName,
                    string.Join(", ", rolesWithUserIds.Where(x => x.UserId == user.Id).Select(x => x.Name))));
            }

            return View(result);
        }

        public IActionResult Roles([FromServices]ExtraAuthorizeDbContext context)
        {
            var roles = context.RolesToPermissions.ToList();
            return View(roles);
        }

        public IActionResult Permissions()
        {
            return View(PermissionDisplay.GetPermissionsToDisplay(typeof(Permissions)));
        }
    }
}
