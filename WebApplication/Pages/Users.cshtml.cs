using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication.Data;

namespace WebApplication.Pages
{
    public class UsersModel : PageModel
    {
        private ApplicationDbContext _applicationDbContext;

        public UsersModel(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }


        public void OnGet()
        {
            var rolesWithUserIds = _applicationDbContext.UserRoles
                .Select(x => new {_applicationDbContext.Roles.Single(y => y.Id == x.RoleId).Name, x.UserId}).ToList();

            var result = new List<Tuple<string, string>>();
            foreach (var user in _applicationDbContext.Users)
            {
                result.Add(new Tuple<string, string>(user.UserName, 
                    string.Join(", ", rolesWithUserIds.Where(x => x.UserId == user.Id).Select(x => x.Name))));
            }

        }
    }
}