using HR.AnsConnector.Features.Studies;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.AnsConnector.Infrastructure.Persistence.Configurations
{
    internal class StudyRecordConfiguration : IEntityTypeConfiguration<StudyRecord>
    {
        public void Configure(EntityTypeBuilder<StudyRecord> builder)
        {
            builder.HasNoKey();

            builder.Ignore(s => s.CreatedAt);
            builder.Ignore(s => s.UpdatedAt);

            builder.Property(s => s.DepartmentId).HasColumnName("department_id").HasConversion<string>();
            builder.Property(s => s.ExternalId).HasColumnName("external_id");
            builder.Property(s => s.Id).HasColumnName("SyncExternalKey").HasConversion<string>();
            builder.Property(s => s.EventId).HasColumnName("SyncEventId");
            builder.Property(s => s.Action).HasColumnName("SyncAction");
        }
    }
}
