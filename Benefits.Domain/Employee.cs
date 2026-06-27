using Benefits.Domain;

namespace Domain
{
    public class Employee : Entity
    {
        public int EmployeeNumber {  get; set; }
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email {  get; set; } = null!;
        public DateTime HireDate {  get; set; }
        public int DepartmentId {  get; set; }
        public Department Department { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        public void Deactivate()
        {
            this.IsActive = false;
        }
    }
}
