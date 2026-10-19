import { TestBed } from "@angular/core/testing";
import { of } from "rxjs";
import { Mock } from "vitest";
import { APP_CONFIG } from "@core/config/app-config.token";
import { EmployeeBenefitDto } from "@core/contracts/employee/employee-benefit.dto";
import { ApiClientService } from "../base/api-client.service";
import { EmployeeEnrollmentsApiService } from "./employee-enrollments-api.service";
import { CreateEnrollmentRequest } from "@core/contracts/enrollment/create-enrollment.request";

describe("EmployeeEnrollmentsApiService", () => {
    let service: EmployeeEnrollmentsApiService;
    let apiGet: Mock<ApiClientService["get"]>;
    let apiPost: Mock<ApiClientService["post"]>;

    const myEnrollmentsUrl = "employees/enrollments";
    const enrollmentsUrl = "enrollments";

    beforeEach(() => {
        apiGet = vi.fn<ApiClientService["get"]>();
        apiPost = vi.fn<ApiClientService["post"]>();

        TestBed.configureTestingModule({
            providers: [
                EmployeeEnrollmentsApiService,
                {
                    provide: ApiClientService,
                    useValue: {
                        get: apiGet,
                        post: apiPost
                    }
                },
                {
                    provide: APP_CONFIG,
                    useValue: {
                        myEnrollmentsUrl,
                        enrollmentsUrl
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
    });

    it("should submit the enrollment request to the configured endpoint", () => {
        const anyBenefitId = 5;
        const anyNonPastDate = "2026-10-31";

        const request: CreateEnrollmentRequest = {
            benefitPlanId: anyBenefitId,
            enrollmentDate: anyNonPastDate
        };

        apiPost.mockReturnValue(of(undefined));

        service.create(request);

        expect(apiPost).toHaveBeenCalledExactlyOnceWith(enrollmentsUrl, request);
    });

    it("should return the observable provided by the API client", () => {
        const anyBenefitId = 5;
        const anyNonPastDate = "2026-10-31";

        const request: CreateEnrollmentRequest = {
            benefitPlanId: anyBenefitId,
            enrollmentDate: anyNonPastDate
        };

        const expectedResponse$ = of(undefined);

        apiPost.mockReturnValue(expectedResponse$);

        const result$ = service.create(request);

        expect(result$).toBe(expectedResponse$);
    });
})