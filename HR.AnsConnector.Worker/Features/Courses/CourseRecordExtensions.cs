namespace HR.AnsConnector.Features.Courses
{
    public static class CourseRecordExtensions
    {
        public static bool IsToBeCreated(this CourseRecord c) => string.Equals("c", c.Action, StringComparison.OrdinalIgnoreCase);
        public static bool IsToBeUpdated(this CourseRecord c) => string.Equals("u", c.Action, StringComparison.OrdinalIgnoreCase);
        public static bool IsToBeDeleted(this CourseRecord c) => string.Equals("d", c.Action, StringComparison.OrdinalIgnoreCase);
    }
}
