import { TestBed } from "@angular/core/testing";
import { of } from "rxjs";
import { Mock } from "vitest";
import { APP_CONFIG } from "@core/config/app-config.token";
import { EmployeeBenefitDto } from "@core/contracts/employee/employee-benefit.dto";
import { ApiClientService } from "../base/api-client.service";
import { EmployeeEnrollmentsApiService } from "./employee-enrollments-api.service";

describe("EmployeeEnrollmentsApiService", () => {
    let service: EmployeeEnrollmentsApiService;
    let apiGet: Mock<ApiClientService["get"]>;

    const myEnrollmentsUrl = "employees/enrollments";

    beforeEach(() => {
        apiGet = vi.fn<ApiClientService["get"]>();

        TestBed.configureTestingModule({
            providers: [
                EmployeeEnrollmentsApiService,
                {
                    provide: ApiClientService,
                    useValue: {
                        get: apiGet
                    }
                },
                {
                    provide: APP_CONFIG,
                    useValue: {
                        myEnrollmentsUrl
                    }
                }
            ]
        });

        service = TestBed.inject(EmployeeEnrollmentsApiService);
    });

    it("should request the current user's enrollments from the configured endpoint", () => {
        const expectedResponse$ = of<EmployeeBenefitDto[]>([]);

        apiGet.mockReturnValue(expectedResponse$);

        service.getMyEnrollments();

        expect(apiGet).toHaveBeenCalledExactlyOnceWith(myEnrollmentsUrl);
    });

    it("should return the Observable provided by the API client", () => {
        const expectedResponse: EmployeeBenefitDto[] = [
            {
                benefitPlanId: 1,
                benefitPlan: "Extended Health Coverage",
                benefitType: "Health",
                enrollmentDate: "2026-01-15",
                endDate: null,
                isActive: true
            },
            {
                benefitPlanId: 2,
                benefitPlan: "Previous Dental Coverage",
                benefitType: "Dental",
                enrollmentDate: "2024-03-01",
                endDate: "2025-12-31",
                isActive: false
            }
        ];
        
        const expectedResponse$ = of(expectedResponse);

        apiGet.mockReturnValue(expectedResponse$);

        const result$ = service.getMyEnrollments();

        expect(result$).toBe(expectedResponse$);
    })
})