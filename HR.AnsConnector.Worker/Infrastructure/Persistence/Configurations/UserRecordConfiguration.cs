using HR.AnsConnector.Features.Users;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.AnsConnector.Infrastructure.Persistence.Configurations
{
    internal class UserRecordConfiguration : IEntityTypeConfiguration<UserRecord>
    {
        private static readonly IDictionary<string, UserRole> roleMappings = new Dictionary<string, UserRole>(StringComparer.OrdinalIgnoreCase)
        {
            { "Student", UserRole.Student },
            { "Teacher", UserRole.Staff },
            { "Staff", UserRole.Staff },
        };

        public void Configure(EntityTypeBuilder<UserRecord> builder)
        {
            builder.HasNoKey();

            builder.Ignore(u => u.CreatedAt);
            builder.Ignore(u => u.UpdatedAt);
            builder.Ignore(u => u.IsActive);
            builder.Ignore(u => u.IsDeleted);
            builder.Ignore(u => u.DeletedAt);
            builder.Ignore(u => u.RoleId);
            builder.Ignore(u => u.DepartmentIds);
            builder.Ignore(u => u.IsAlumni);

            builder.Property(u => u.Role).HasConversion(
                convertToProviderExpression: (UserRole? role) => role != null ? role.ToString() : null,
                convertFromProviderExpression: (string? role) => !string.IsNullOrEmpty(role) && roleMappings.ContainsKey(role) ? roleMappings[role] : null);

            builder.Property(u => u.UniqueId).HasColumnName("Uid");
            builder.Property(u => u.Id).HasColumnName("SyncExternalKey").HasConversion<string>();
            builder.Property(u => u.EventId).HasColumnName("SyncEventId");
            builder.Property(u => u.Action).HasColumnName("SyncAction");
        }
    }
}
