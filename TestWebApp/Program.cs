using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TestWebApp.StartupCode;

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

            webHost.SetupDatabases();
            return webHost;
        }
    }
}
