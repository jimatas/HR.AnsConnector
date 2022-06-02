namespace HR.AnsConnector.Features.Studies
{
    public static class StudyRecordExtensions
    {
        public static bool IsToBeCreated(this StudyRecord s) => string.Equals("c", s.Action, StringComparison.OrdinalIgnoreCase);
        public static bool IsToBeUpdated(this StudyRecord s) => string.Equals("u", s.Action, StringComparison.OrdinalIgnoreCase);
        public static bool IsToBeDeleted(this StudyRecord s) => string.Equals("d", s.Action, StringComparison.OrdinalIgnoreCase);
    }
}
