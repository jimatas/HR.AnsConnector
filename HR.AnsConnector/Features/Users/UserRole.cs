namespace HR.AnsConnector.Features.Users
{
    public enum UserRole
    {
        /// <summary>
        /// A school administrator.
        /// </summary>
        Owner = 1,

        /// <summary>
        /// A department admin (needs to be assigned to a department).
        /// </summary>
        Operator,

        /// <summary>
        /// An uploader.
        /// </summary>
        Uploader,

        /// <summary>
        /// Staff is the base role for employees.
        /// </summary>
        Staff,

        /// <summary>
        /// Student is the base role for students.
        /// </summary>
        Student
    }
}
