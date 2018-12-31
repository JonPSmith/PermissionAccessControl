// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using DataAuthorize;
using DataLayer.EfClasses.MultiTenantClasses;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfCode
{
    public class MultiTenantDbContext : DbContext
    {
        private readonly int _shopKey;

        public DbSet<MultiTenantUser> MultiTenantUsers { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<StockInfo> CurrentStock { get; set; }

        public MultiTenantDbContext(DbContextOptions<MultiTenantDbContext> options, IGetClaimsProvider userData)
            : base(options)
        {
            _shopKey = userData.ShopKey;
        }

        //I only have to override these two version of SaveChanges, as the other two versions call these
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.MarkCreatedItemWithShopKey(_shopKey);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            this.MarkCreatedItemWithShopKey(_shopKey);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MultiTenantUser>().HasQueryFilter(x => x.ShopKey == _shopKey);
            modelBuilder.Entity<Shop>().HasQueryFilter(x => x.ShopKey == _shopKey);
            modelBuilder.Entity<StockInfo>().HasQueryFilter(x => x.ShopKey == _shopKey);
        }
    }
}