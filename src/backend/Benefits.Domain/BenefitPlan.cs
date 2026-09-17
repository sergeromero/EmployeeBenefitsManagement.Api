namespace Benefits.Domain
{
    public class BenefitPlan : Entity
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public bool IsActive { get; set; } = true;
        public int BenefitTypeId { get; set; }
        public BenefitType BenefitType { get; set; } = null!;
        public int EnrollmentCategoryId { get; set; }
        public EnrollmentCategory EnrollmentCategory { get; set; } = null!;
        public IReadOnlyCollection<EmployeeEnrollment> Enrollments { get; set; } = [];

        public void Deactivate()
        {
            this.IsActive = false;
        }
    }
}
