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
        public ICollection<EmployeeEnrollment> Enrollments { get; set; } = new List<EmployeeEnrollment>();

        public string? UserId { get; set; } = null!;

        public void Deactivate()
        {
            IsActive = false;
        }
    }
}
