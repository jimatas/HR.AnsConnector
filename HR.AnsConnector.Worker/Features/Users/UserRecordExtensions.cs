namespace HR.AnsConnector.Features.Users
{
    public static class UserRecordExtensions
    {
        public static bool IsToBeCreated(this UserRecord u) => string.Equals("c", u.Action, StringComparison.OrdinalIgnoreCase);
        public static bool IsToBeUpdated(this UserRecord u) => string.Equals("u", u.Action, StringComparison.OrdinalIgnoreCase);
        public static bool IsToBeDeleted(this UserRecord u) => string.Equals("d", u.Action, StringComparison.OrdinalIgnoreCase);
    }
}
