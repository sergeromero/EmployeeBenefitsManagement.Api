namespace Benefits.Domain
{
    public class Employee : Entity
    {
        public string EmployeeNumber {  get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email {  get; set; } = null!;
        public DateOnly HireDate {  get; set; }
        public int DepartmentId {  get; set; }
        public Department Department { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public IReadOnlyCollection<EmployeeEnrollment> Enrollments { get; set; } = [];

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
