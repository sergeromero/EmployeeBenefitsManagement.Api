namespace Benefits.Domain
{
    public sealed class EmployeeEnrollment
    {
        public int EmployeeId { get; set; }
        public int BenefitPlanId { get; set; }
        public DateOnly EnrollmentDate { get; set; }
        public Employee Employee { get; set; } = null!;
        public BenefitPlan BenefitPlan { get; set; } = null!;
    }
}
