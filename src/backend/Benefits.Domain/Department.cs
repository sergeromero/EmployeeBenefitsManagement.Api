namespace Benefits.Domain
{
    public class Department : Entity
    {
        public string Name { get; set; } = null!;

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
