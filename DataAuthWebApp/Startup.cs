using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataAuthorize;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataAuthWebApp.Data;
using DataLayer.EfCode;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RolesToPermission;
using StartupCode;

namespace DataAuthWebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //Have own database for roles to Permissions and Modules - uses in-memory database
            var authConnection = SetupSqliteInMemoryConnection();
            services.AddDbContext<ExtraAuthorizeDbContext>(options => options.UseSqlite(authConnection));
            var bizConnection = SetupSqliteInMemoryConnection();
            services.AddDbContext<BusinessDbContext>(options => options.UseSqlite(bizConnection));

            //Swapped over to Sqlite in-memory database for identity database
            var identityConnection = SetupSqliteInMemoryConnection();
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(identityConnection));
            //services.AddDbContext<ApplicationDbContext>(options =>
            //    options.UseSqlServer(
            //        Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDefaultIdentity<IdentityUser>()
            //    .AddEntityFrameworkStores<ApplicationDbContext>();
            //NOTE: Had to use AddIdentity<IdentityUser, IdentityRole>() rather than AddDefaultIdentity<IdentityUser> to get roles working
            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            //We build the AuthCookie's OnValidatePrincipal 
            var sp = services.BuildServiceProvider();
            var extraAuthDbContextOptions = sp.GetRequiredService<DbContextOptions<ExtraAuthorizeDbContext>>();
            var authCookieValidate = new AuthCookieValidate(new CalcAllowedPermissions(extraAuthDbContextOptions));

            //see https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-configuration?view=aspnetcore-2.1#cookie-settings
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnValidatePrincipal = authCookieValidate.ValidateAsync;
            });

            //Register the Permission policy handlers
            services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProvider>();
            services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

            //This is needed by BusinessDbContext to get the userId from claims
            services.AddScoped<IUserIdProvider, UserIdFromClaims>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        //ADDED
        private static SqliteConnection SetupSqliteInMemoryConnection()
        {
            var connectionStringBuilder = new SqliteConnectionStringBuilder { DataSource = ":memory:" };
            var connectionString = connectionStringBuilder.ToString();
            var connection = new SqliteConnection(connectionString);
            connection.Open();  //see https://github.com/aspnet/EntityFramework/issues/6968
            return connection;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            //The users can only be set up after app.UseAuthentication() is called
            serviceProvider.AddUsersAndExtraAuthAsync().Wait();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
