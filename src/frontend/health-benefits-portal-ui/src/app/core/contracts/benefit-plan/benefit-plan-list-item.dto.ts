import { BenefitTypeDto } from "./benefit-type.dto";

export type EnrollmentIneligibilityReason = 
| "AlreadyEnrolledInBenefit"
| "AlreadyEnrolledInCategory";

export interface EnrollmentEligibilityDto {
    canEnroll: boolean,
    reason: EnrollmentIneligibilityReason | null
}

export interface BenefitPlanListItemDto {
    id: number;
    name: string;
    description: string;
    benefitType: BenefitTypeDto;
    enrollmentCategoryName: string;
    enrollmentEligibility: EnrollmentEligibilityDto;
}