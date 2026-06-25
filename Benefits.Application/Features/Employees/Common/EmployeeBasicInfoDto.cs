using System;
using System.Collections.Generic;
using System.Text;

namespace Benefits.Application.Features.Employees.Common
{
    public class EmployeeBasicInfoDto
    {
        public int Id { get; set; }
        public int EmployeeNumber { get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public DateTime HireDate { get; set; }
        public int DepartmentId { get; set; }
    }
}
