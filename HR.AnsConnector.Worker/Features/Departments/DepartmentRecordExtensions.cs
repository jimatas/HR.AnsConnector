namespace HR.AnsConnector.Features.Departments
{
    public static class DepartmentRecordExtensions
    {
        public static bool IsToBeCreated(this DepartmentRecord d) => string.Equals("c", d.Action, StringComparison.OrdinalIgnoreCase);
        public static bool IsToBeUpdated(this DepartmentRecord d) => string.Equals("u", d.Action, StringComparison.OrdinalIgnoreCase);
        public static bool IsToBeDeleted(this DepartmentRecord d) => string.Equals("d", d.Action, StringComparison.OrdinalIgnoreCase);
    }
}
