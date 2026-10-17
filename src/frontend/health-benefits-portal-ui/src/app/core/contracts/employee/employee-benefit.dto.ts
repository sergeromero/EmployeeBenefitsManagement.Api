export interface EmployeeBenefitDto {
    benefitPlanId: number;
    benefitPlan: string;
    benefitType: string;
    enrollmentDate: string;
    endDate: string | null;
    isActive: boolean;
}