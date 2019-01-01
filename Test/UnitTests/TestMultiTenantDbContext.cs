// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT licence. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using DataLayer.EfClasses.MultiTenantClasses;
using DataLayer.EfCode;
using Microsoft.EntityFrameworkCore;
using Test.Mocks;
using TestSupport.EfHelpers;
using Xunit;
using Xunit.Extensions.AssertExtensions;

namespace Test.UnitTests
{
    public class TestMultiTenantDbContext
    {


        [Fact]
        public void TestCreateValidDatabaseOk()
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<MultiTenantDbContext>();
            using (var context = new MultiTenantDbContext(options, new MockGetClaimsProvider("user-id", 0, null)))
            {
                context.Database.EnsureCreated();

                //ATTEMPT
                var shop1 = new Shop {Name = "Test1"};
                var shop2 = new Shop {Name = "Test2"};
                context.AddRange(shop1, shop2);
                context.SaveChanges();

                //VERIFY
                context.Shops.IgnoreQueryFilters().Count().ShouldEqual(2);
            }
        }

        [Theory]
        [InlineData(1, null, "shop2")]
        [InlineData(123, "manager-id", "shop1")]
        public void TestShopHierarchicalFilterOk(int shopKey, string districtManagerId, string stockName)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<MultiTenantDbContext>();
            using (var context =
                new MultiTenantDbContext(options, new MockGetClaimsProvider("user-id", 0, "manager-id")))
            {
                context.Database.EnsureCreated();

                var mUser = new MultiTenantUser
                {
                    UserId = "manager-id",
                    IsDistrictManager = true
                };
                var shop1 = new Shop {Name = "shop1", DistrictManager = mUser};
                var shop2 = new Shop {Name = "shop2"};
                context.AddRange(shop1, shop2);
                context.SaveChanges();
                var stock1 = new StockInfo
                    {Name = shop1.Name, NumInStock = 10, AtShop = shop1, DistrictManagerId = shop1.DistrictManagerId};
                var stock2 = new StockInfo
                    {Name = shop2.Name, NumInStock = 10, AtShop = shop2, DistrictManagerId = shop2.DistrictManagerId};
                context.AddRange(stock1, stock2);
                context.SaveChanges();
            }
            using (var context = new MultiTenantDbContext(options, new MockGetClaimsProvider("user-id", shopKey, districtManagerId)))
            {

                //ATTEMPT
                var filtered = context.CurrentStock.ToList();

                //VERIFY
                filtered.Count.ShouldEqual(1);
                filtered.Single().Name.ShouldEqual(stockName);
            }
        }

        [Theory]
        [InlineData(1, null, "shop2")]
        [InlineData(123, "manager-id", "shop1")]
        public void TestShopHierarchicalFilterWithIncludeOk(int shopKey, string districtManagerId, string stockName)
        {
            //SETUP
            var options = SqliteInMemory.CreateOptions<MultiTenantDbContext>();
            using (var context =
                new MultiTenantDbContext(options, new MockGetClaimsProvider("user-id", 0, "manager-id")))
            {
                context.Database.EnsureCreated();

                var mUser = new MultiTenantUser
                {
                    UserId = "manager-id",
                    IsDistrictManager = true
                };
                var shop1 = new Shop { Name = "shop1", DistrictManager = mUser };
                var shop2 = new Shop { Name = "shop2" };
                context.AddRange(shop1, shop2);
                context.SaveChanges();
                var stock1 = new StockInfo
                    { Name = shop1.Name, NumInStock = 10, AtShop = shop1, DistrictManagerId = shop1.DistrictManagerId };
                var stock2 = new StockInfo
                    { Name = shop2.Name, NumInStock = 10, AtShop = shop2, DistrictManagerId = shop2.DistrictManagerId };
                context.AddRange(stock1, stock2);
                context.SaveChanges();
            }
            using (var context = new MultiTenantDbContext(options, new MockGetClaimsProvider("user-id", shopKey, districtManagerId)))
            {

                //ATTEMPT
                var filtered = context.CurrentStock.Include(x => x.AtShop).ToList();

                //VERIFY
                filtered.Count.ShouldEqual(1);
                filtered.Single().Name.ShouldEqual(stockName);
            }
        }
    }

}