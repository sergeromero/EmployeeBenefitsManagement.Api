namespace Benefits.Application.Features.Employees.Queries.ViewEmployeeBenefits
{
    public sealed record EmployeeBenefitsDto(
        int Id,
        string FirstName,
        string LastName,
        string Department,
        IReadOnlyList<EmployeeBenefitDto> Benefits);
}
