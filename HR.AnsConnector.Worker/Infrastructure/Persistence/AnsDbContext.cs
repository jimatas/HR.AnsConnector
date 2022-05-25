using HR.AnsConnector.Features.Departments;
using HR.AnsConnector.Features.Users;

using Microsoft.EntityFrameworkCore;

namespace HR.AnsConnector.Infrastructure.Persistence
{
    public class AnsDbContext : DbContext
    {
        public AnsDbContext() { }
        public AnsDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(builder);
        }

        public DbSet<UserRecord> Users => Set<UserRecord>();
        public DbSet<DepartmentRecord> Departments => Set<DepartmentRecord>();
    }
}
