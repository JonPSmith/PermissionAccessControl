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
        public int ShopKey { get; }
        public string DistrictManagerId { get; }

        public DbSet<MultiTenantUser> MultiTenantUsers { get; set; }
        public DbSet<Shop> Shops { get; set; }
        public DbSet<StockInfo> CurrentStock { get; set; }

        public MultiTenantDbContext(DbContextOptions<MultiTenantDbContext> options, IGetClaimsProvider userData)
            : base(options)
        {
            ShopKey = userData.ShopKey;
            DistrictManagerId = userData.DistrictManagerId;
        }

        //I only have to override these two version of SaveChanges, as the other two versions call these
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.MarkCreatedItemWithShopKey(ShopKey);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            this.MarkCreatedItemWithShopKey(ShopKey);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Standard multi-tenant query filter - just filter on ShopKey
            //This assumes that the district manager cannot manage the shop users
            modelBuilder.Entity<MultiTenantUser>().HasQueryFilter(x => x.ShopKey == ShopKey);

            //Altered query filter to handle hierarchical access
            modelBuilder.Entity<Shop>().HasQueryFilter(x => DistrictManagerId == null
                ? x.ShopKey == ShopKey
                : x.DistrictManagerId == DistrictManagerId);
            modelBuilder.Entity<StockInfo>().HasQueryFilter(x => DistrictManagerId == null 
                ? x.ShopKey == ShopKey
                : x.DistrictManagerId == DistrictManagerId);
        }
    }
}