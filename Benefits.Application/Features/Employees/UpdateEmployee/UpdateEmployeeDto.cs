namespace Benefits.Application.Features.Employees.UpdateEmployee
{
    public class UpdateEmployeeDto
    {
        public int EmployeeNumber { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime HireDate { get; set; }
        public int DepartmentId { get; set; }
    }
}
