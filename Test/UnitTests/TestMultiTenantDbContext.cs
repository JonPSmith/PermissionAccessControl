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
            using (var context = new MultiTenantDbContext(options, new MockGetClaimsProvider("user-id", 0)))
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
  

    }

}