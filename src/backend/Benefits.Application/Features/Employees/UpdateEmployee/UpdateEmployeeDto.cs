namespace Benefits.Application.Features.Employees.UpdateEmployee
{
    public class UpdateEmployeeDto
    {
        public string EmployeeNumber { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateOnly HireDate { get; set; }
        public int DepartmentId { get; set; }
    }
}
