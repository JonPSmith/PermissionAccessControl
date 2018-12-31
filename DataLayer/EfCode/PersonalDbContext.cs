// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataAuthorize;
using DataLayer.EfClasses.BusinessClasses;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.EfCode
{
    public class PersonalDbContext : DbContext
    {
        private readonly string _userId;

        public DbSet<PersonalData> PersonalDatas { get; set; }

        public PersonalDbContext(DbContextOptions<PersonalDbContext> options, IGetClaimsProvider userData)
            : base(options)
        {
            _userId = userData.UserId;
        }

        //I only have to override these two version of SaveChanges, as the other two versions call these
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.MarkCreatedItemAsOwnedBy(_userId);
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new CancellationToken())
        {
            this.MarkCreatedItemAsOwnedBy(_userId);
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var entityOwnedBy in modelBuilder.Model.GetEntityTypes().Where(x => x.ClrType.GetInterface(nameof(IOwnedBy)) != null))
            {
                modelBuilder.Entity(entityOwnedBy.ClrType).HasIndex(nameof(IOwnedBy.OwnedBy));
            }

            modelBuilder.Entity<PersonalData>().HasQueryFilter(x => x.OwnedBy == _userId);
        }
    }
}