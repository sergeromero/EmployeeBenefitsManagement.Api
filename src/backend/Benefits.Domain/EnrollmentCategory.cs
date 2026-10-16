namespace Benefits.Domain
{
    public sealed class EnrollmentCategory : Entity
    {
        public string Name { get; set; } = null!;
        public ICollection<BenefitPlan> BenefitPlans { get; set; } = new List<BenefitPlan>();
    }
}
