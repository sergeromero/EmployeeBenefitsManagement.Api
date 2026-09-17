namespace Benefits.Domain
{
    public sealed class EmployeeEnrollment
    {
        public int EmployeeId { get; set; }
        public int BenefitPlanId { get; set; }
        public DateOnly EnrollmentDate { get; set; }
        public DateOnly? EndDate { get; private set; }
        public Employee Employee { get; set; } = null!;
        public BenefitPlan BenefitPlan { get; set; } = null!;

        public bool IsActive => EndDate is null;

        public void EndEnrollment(DateOnly endDate)
        {
            EndDate = endDate;
        }
    }
}
