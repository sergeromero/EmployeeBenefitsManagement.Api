namespace Benefits.Application.Features.Employees.Queries.ViewEmployeeBenefits
{
    public sealed record EmployeeBenefitDto(
        int BenefitPlanId,
        string BenefitPlan,
        string BenefitType);
}
