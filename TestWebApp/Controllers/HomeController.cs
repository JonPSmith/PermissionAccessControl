using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfCode;
using Microsoft.AspNetCore.Mvc;
using TestWebApp.Data;
using TestWebApp.DisplayCode;
using TestWebApp.Models;

namespace TestWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index([FromServices]ApplicationDbContext applicationDbContext,
                                   [FromServices] ExtraAuthorizeDbContext extraAuthorizeDbContext)
        {
            var userLister = new ListUsers(applicationDbContext, extraAuthorizeDbContext);
            var roleLister = new ListRoles(extraAuthorizeDbContext);

            return View(new HomePageDto(userLister.ListUserWithRolesAndModules(), roleLister.ListRolesWithPermissionsExplained().ToList()));
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
