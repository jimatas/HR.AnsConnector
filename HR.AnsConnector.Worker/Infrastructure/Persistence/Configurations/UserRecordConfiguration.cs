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
        };

        public void Configure(EntityTypeBuilder<UserRecord> builder)
        {
            builder.HasNoKey();

            builder.Ignore(u => u.Id)
                .Ignore(u => u.CreatedAt)
                .Ignore(u => u.UpdatedAt)
                .Ignore(u => u.IsActive)
                .Ignore(u => u.IsDeleted)
                .Ignore(u => u.DeletedAt)
                .Ignore(u => u.RoleId)
                .Ignore(u => u.DepartmentIds)
                .Ignore(u => u.IsAlumni);

            _ = builder.Property(u => u.Role).HasConversion(
                convertToProviderExpression: (UserRole? role) => null, // No need to map back to provider type.
                convertFromProviderExpression: (string? role) => !string.IsNullOrEmpty(role) && roleMappings.ContainsKey(role) ? roleMappings[role] : null);

            builder.Property(u => u.EventId).HasColumnName("SyncEventId");
            builder.Property(u => u.Action).HasColumnName("SyncAction");
        }
    }
}
