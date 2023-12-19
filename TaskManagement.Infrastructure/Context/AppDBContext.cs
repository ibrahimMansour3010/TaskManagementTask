using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using TaskManagement.Domain.Common;
using TaskManagement.Domain.Identity;

namespace TaskManagement.Infrastructure.Context
{
    public class AppDBContext:IdentityDbContext<ApplicationUser,ApplicationRole,string>
    {
        public DbSet<TaskManagement.Domain.Models.Task> Tasks { get; set; }
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }

        public void AddCreatedBy(string userId) {
            var now = DateTime.UtcNow;

            foreach (var changedEntity in ChangeTracker.Entries())
            {
                if (changedEntity.Entity is BaseEntityModel entity)
                {
                    switch (changedEntity.State)
                    {
                        case EntityState.Added:
                            entity.CreationDate = now;
                            entity.UpdatedDate = now;
                            entity.CreatedBy = userId;
                            entity.UpdatedBy = userId;
                            break;
                        case EntityState.Modified:
                            base.Entry(entity).Property(x => x.CreatedBy).IsModified = false;
                            base.Entry(entity).Property(x => x.CreationDate).IsModified = false;
                            entity.UpdatedDate = now;
                            entity.UpdatedBy = userId;
                            break;
                    }
                }
            }
        }
    }
}
