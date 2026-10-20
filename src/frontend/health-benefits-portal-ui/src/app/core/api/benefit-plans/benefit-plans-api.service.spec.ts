import { TestBed } from "@angular/core/testing";
import { of } from "rxjs";
 import { APP_CONFIG } from "@core/config/app-config.token";
 import { BenefitPlanListItemDto } from "@core/contracts/benefit-plan/benefit-plan-list-item.dto";
 import { ApiClientService } from "../base/api-client.service";
 import { BenefitPlansApiService } from "./benefit-plans-api.service";
import { Mock } from "vitest";

 describe("BenefitPlansApiService", () => {
    let service: BenefitPlansApiService;
    let apiGet: Mock<ApiClientService["get"]>;

    const benefitPlansUrl = "benefitplans";

    beforeEach(() => {
        apiGet = vi.fn<ApiClientService["get"]>();

        TestBed.configureTestingModule({
            providers: [
                BenefitPlansApiService,
                {
                    provide: ApiClientService,
                    useValue: {
                        get: apiGet
                    }
                },
                {
                    provide: APP_CONFIG,
                    useValue: {
                        benefitPlansUrl
                    }
                }
            ]
        });

        service = TestBed.inject(BenefitPlansApiService);
    });

    it("should request all benefit plans from the configured endpoint", () => {
        const expectedResponse$ = of<BenefitPlanListItemDto[]>([]);

        apiGet.mockReturnValue(expectedResponse$);

        service.getAll();

        expect(apiGet).toHaveBeenCalledExactlyOnceWith(benefitPlansUrl);
    });

    it("should return the Observable provided by the API client", () => {
        const expectedResponse: BenefitPlanListItemDto[] = [
            {
                id: 1,
                name: "Extended Health Coverage",
                description: "Coverage for eligible healthcare expenses.",
                benefitType: {
                    id: 1,
                    name: "Health"
                },
                enrollmentCategoryName: "Health Benefits",
                enrollmentEligibility: {
                    canEnroll: true,
                    reason: null
                }
            }            
        ];

        const expectedResponse$ = of(expectedResponse);

        apiGet.mockReturnValue(expectedResponse$);

        const result$ = service.getAll();

        expect(result$).toBe(expectedResponse$);
    });
 });