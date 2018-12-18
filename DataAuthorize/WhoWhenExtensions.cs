// Copyright (c) 2018 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DataAuthorize
{
    public static class WhoWhenExtensions
    {
        /// <summary>
        /// This is called in the overridden SaveChanges in the application's DbContext
        /// Its job is to call the LogChanges of any entities being added or updated that have the IWhoWhen interface
        /// </summary>
        /// <param name="context"></param>
        /// <param name="userId"></param>
        public static void MarkWhoWhenEntities(this DbContext context, string userId)
        {
            foreach (var entityEntry in context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
            {
                if (entityEntry.Entity is IWhoWhen whoWhenEntity)
                {
                    whoWhenEntity.LogChange(entityEntry.State == EntityState.Added, userId);
                }
            }
        }
    }
}