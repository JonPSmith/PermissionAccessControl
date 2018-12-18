// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Threading;
using System.Threading.Tasks;
using DataAuthorize;
using DataLayer.EfClasses.BusinessClasses;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfCode
{
    public class BusinessDbContext : DbContext
    {
        private readonly string _userId;

        public DbSet<PersonalData> PersonalDatas { get; set; }


        public BusinessDbContext(DbContextOptions<BusinessDbContext> options, IUserIdProvider userData)
            : base(options)
        {
            _userId = userData.UserId;
        }

        //I only have to override these two version of SaveChanges, as the other two versions call these
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.MarkWhoWhenEntities(_userId);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            this.MarkWhoWhenEntities(_userId);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
    }
}