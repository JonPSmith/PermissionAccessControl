// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayer.EfClasses.AuthClasses;
using DataLayer.EfClasses.BusinessClasses;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfCode
{
    /// <summary>
    /// This is a cheat DB context that I use to create a database that both ExtraAuthorizeDbContext and BusinessDbContext can access
    /// </summary>
    public class AuthAndBizDbContext : DbContext
    {
        //ExtraAuthorizeDbContext
        public DbSet<RoleToPermissions> RolesToPermissions { get; set; }
        public DbSet<ModulesForUser> ModulesForUsers { get; set; }

        //BusinessDbContext
        public DbSet<PersonalData> PersonalDatas { get; set; }


        public AuthAndBizDbContext(DbContextOptions<AuthAndBizDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleToPermissions>().Property<string>("_permissionsInRole")
                .IsUnicode(false).HasColumnName("PermissionsInRole").IsRequired();
        }
    }
}