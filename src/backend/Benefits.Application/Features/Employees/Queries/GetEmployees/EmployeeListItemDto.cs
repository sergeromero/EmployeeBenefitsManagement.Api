namespace Benefits.Application.Features.Employees.Queries.GetEmployees
{
    public sealed class EmployeeListItemDto
    {
        public int Id { get; set; } = default;
        public string EmployeeNumber { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public DateOnly HireDate { get; set; }
        public string DepartmentName { get; set; } = default!;
        public string? UserName { get; set; }
        public bool? HasUser { get; set; }
    }
}
