using System.Text.Json.Serialization;

namespace Benefits.Application.Features.BenefitPlans.Queries.GetBenefitPlans
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum EnrollmentIneligibilityReason
    {
        AlreadyEnrolledInBenefit,
        AlreadyEnrolledInCategory
    }

    public sealed record EnrollmentEligibilityDto(bool CanEnroll, EnrollmentIneligibilityReason? Reason);
}
