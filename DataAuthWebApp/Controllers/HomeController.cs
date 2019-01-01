using System.Diagnostics;
using System.Linq;
using DataAuthWebApp.Data;
using DataAuthWebApp.DisplayCode;
using Microsoft.AspNetCore.Mvc;
using DataAuthWebApp.Models;
using DataLayer.EfCode;

namespace DataAuthWebApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index([FromServices]ApplicationDbContext applicationDbContext,
            [FromServices] MultiTenantDbContext extraAuthorizeDbContext)
        {
            var userLister = new ListUsers(applicationDbContext, extraAuthorizeDbContext);

            return View(userLister.ListUserWithRolesAndModules());
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
