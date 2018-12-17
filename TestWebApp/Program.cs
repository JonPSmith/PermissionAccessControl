using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.EfCode;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TestWebApp.Data;
using TestWebApp.RolesToPermissions;

namespace TestWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHostAsync(args).Run();
        }

        private static IWebHost BuildWebHostAsync(string[] args)
        {
            var webHost = WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

            //Because I am using in-memory databases I need to make sure they are created 
            //before my startup code tries to use them
            SetupDatabases(webHost);
            return webHost;
        }


        private static void SetupDatabases(IWebHost webHost)
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                using (var context = services.GetRequiredService<ApplicationDbContext>())
                {
                    context.Database.EnsureCreated();
                }
                using (var context = services.GetRequiredService<ExtraAuthorizeDbContext>())
                {
                    context.Database.EnsureCreated();
                }
            }
        }
    }
}
