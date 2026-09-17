namespace Benefits.Domain.Constants
{
    public static class Roles
    {
        public const string Administrator = "Administrator";
        public const string HR = "HR";
        public const string Employee = "Employee";

        public static IReadOnlyList<string> All { get; } = [
            Administrator,
            HR,
            Employee
        ];
    }
}

