using HR.AnsConnector.Features.Departments;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.AnsConnector.Infrastructure.Persistence.Configurations
{
    internal class DepartmentRecordConfiguration : IEntityTypeConfiguration<DepartmentRecord>
    {
        public void Configure(EntityTypeBuilder<DepartmentRecord> builder)
        {
            builder.HasNoKey();

            builder.Ignore(d => d.CreatedAt);
            builder.Ignore(d => d.UpdatedAt);
            builder.Ignore(d => d.OperatorIds);

            builder.Property(d => d.ExternalId).HasColumnName("external_id");
            builder.Property(d => d.Id).HasColumnName("SyncExternalKey").HasConversion<string>();
            builder.Property(d => d.EventId).HasColumnName("SyncEventId");
            builder.Property(d => d.Action).HasColumnName("SyncAction");
        }
    }
}
