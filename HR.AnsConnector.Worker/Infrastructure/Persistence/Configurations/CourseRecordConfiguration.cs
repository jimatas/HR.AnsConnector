using HR.AnsConnector.Features.Courses;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HR.AnsConnector.Infrastructure.Persistence.Configurations
{
    internal class CourseRecordConfiguration : IEntityTypeConfiguration<CourseRecord>
    {
        public void Configure(EntityTypeBuilder<CourseRecord> builder)
        {
            builder.HasNoKey();

            builder.Ignore(c => c.CreatedAt);
            builder.Ignore(c => c.UpdatedAt);
            builder.Ignore(c => c.DeletedAt);

            builder.Property(c => c.StudyIds).HasColumnName("study_id").HasConversion(
                convertToProviderExpression: (IEnumerable<int> studyIds) => studyIds.Any() ? studyIds.First().ToString() : null,
                convertFromProviderExpression: (string? studyId) => string.IsNullOrEmpty(studyId) ? Enumerable.Empty<int>() : new[] { int.Parse(studyId) },
                valueComparer: new ValueComparer<IEnumerable<int>>(
                    equalsExpression: (x, y) => (x != null && y != null && x.SequenceEqual(y)) || x == y,
                    hashCodeExpression: studyIds => studyIds.Aggregate(0, (hashCode, id) => HashCode.Combine(hashCode, id)),
                    snapshotExpression: studyIds => studyIds.ToHashSet()));

            builder.Property(c => c.IsDeleted).HasColumnName("trashed").HasConversion<int>();
            builder.Property(c => c.Year).HasConversion<short>();
            builder.Property(c => c.CourseCode).HasColumnName("course_code");
            builder.Property(c => c.SelfEnroll).HasColumnName("self_enroll").HasConversion<int>();
            builder.Property(c => c.ExternalId).HasColumnName("external_id");
            builder.Property(c => c.Id).HasColumnName("SyncExternalKey").HasConversion<string>();
            builder.Property(c => c.EventId).HasColumnName("SyncEventId");
            builder.Property(c => c.Action).HasColumnName("SyncAction");
        }
    }
}
