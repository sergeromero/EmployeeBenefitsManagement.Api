namespace Benefits.Domain
{
    public sealed class EmployeeProvisioningProcess
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string? UserId { get; set; }
        public int? EmployeeId { get; set; }
        public ProvisioningStatus Status { get; set; }
        public string? Error { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
