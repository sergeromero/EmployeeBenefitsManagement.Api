namespace Benefits.Application.Features.Employees.Common
{
    public class EmployeeBasicInfoDto
    {
        public int Id { get; set; }
        public string EmployeeNumber { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateOnly HireDate { get; set; }
        public int DepartmentId { get; set; }
    }
}
