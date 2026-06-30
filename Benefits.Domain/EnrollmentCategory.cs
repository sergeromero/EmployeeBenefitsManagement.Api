namespace Benefits.Domain
{
    public sealed class EnrollmentCategory : Entity
    {
        public string Name { get; set; } = null!;
        public IReadOnlyCollection<BenefitPlan> BenefitPlans { get; set; } = [];
    }
}
