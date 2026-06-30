namespace Benefits.Domain
{
    public class Department : Entity
    {
        public string Name { get; set; } = null!;

        public IReadOnlyCollection<Employee> Employees { get; set; } = [];
    }
}
