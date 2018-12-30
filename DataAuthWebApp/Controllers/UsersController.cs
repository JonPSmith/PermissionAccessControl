using System.Linq;
using DataAuthWebApp.Data;
using DataAuthWebApp.DisplayCode;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Mvc;
using PermissionParts;

namespace DataAuthWebApp.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ExtraAuthorizeDbContext _extraAuthorizeDbContext;
        private readonly MultiTenantDbContext _multiTenantDbContext;

        public UsersController(ApplicationDbContext applicationDbContext, ExtraAuthorizeDbContext extraAuthorizeDbContext, MultiTenantDbContext multiTenantDbContext)
        {
            _applicationDbContext = applicationDbContext;
            _extraAuthorizeDbContext = extraAuthorizeDbContext;
            _multiTenantDbContext = multiTenantDbContext;
        }

        public IActionResult Index()
        {
            return View(HttpContext.User);
        }

        public IActionResult List()
        {
            var lister = new ListUsers(_applicationDbContext, _multiTenantDbContext);

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
