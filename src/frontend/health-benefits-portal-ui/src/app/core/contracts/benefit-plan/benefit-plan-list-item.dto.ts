import { BenefitTypeDto } from "./benefit-type.dto";

export interface BenefitPlanListItemDto {
    id: number;
    name: string;
    description: string;
    benefitType: BenefitTypeDto;
    enrollmentCategoryName: string
}