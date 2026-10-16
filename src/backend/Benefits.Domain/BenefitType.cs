namespace Benefits.Domain
{
    public class BenefitType : Entity
    {
        public string Name { get; set; } = null!;
        public ICollection<BenefitPlan> BenefitPlans { get; set; } = new List<BenefitPlan>();
    }
}
