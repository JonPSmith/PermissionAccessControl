// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.EntityFrameworkCore;

namespace TestWebApp.Data
{
    public class RolesDbContext : DbContext
    {
        public DbSet<RoleToPermissions> RolesToPermissions { get; set; }

        public RolesDbContext(DbContextOptions<RolesDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RoleToPermissions>().Property<string>("_permissionsInRole")
                .IsUnicode(false).HasColumnName("PermissionsInRole").IsRequired();
        }
    }
}