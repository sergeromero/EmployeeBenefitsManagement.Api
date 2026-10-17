import { ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { Mock } from 'vitest';
import { Observable, of, Subject, throwError } from 'rxjs';
import { EmployeeEnrollmentsApiService } from '@core/api/enrollments/employee-enrollments-api.service';
import { EmployeeBenefitDto } from '@core/contracts/employee/employee-benefit.dto';
import { MyEnrollmentsComponent } from './my-enrollments.component';

describe('MyEnrollmentsComponentTsComponent', () => {
  let component: MyEnrollmentsComponent;
  let fixture: ComponentFixture<MyEnrollmentsComponent>;

  let getMyEnrollments: Mock<EmployeeEnrollmentsApiService["getMyEnrollments"]>;

  const enrollments: EmployeeBenefitDto[] = [
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
          benefitPlan: "Dental Coverage",
          benefitType: "Dental",
          enrollmentDate: "2025-03-10",
          endDate: "2026-02-28",
          isActive: false
      }
  ];

  function createComponent(): void {
    fixture = TestBed.createComponent(MyEnrollmentsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }

  beforeEach(async () => {
    getMyEnrollments = vi.fn<EmployeeEnrollmentsApiService["getMyEnrollments"]>();

    await TestBed.configureTestingModule({
      imports: [MyEnrollmentsComponent],
      providers: [
        {
          provide: EmployeeEnrollmentsApiService,
          useValue: {
            getMyEnrollments
          }
        }
      ]
    }).compileComponents();
  });

  describe("initialization", () => {
    it('should create', () => {
      getMyEnrollments.mockReturnValue(of());
      createComponent();
      expect(component).toBeTruthy();
    });

    it("should request the current user's enrollments when initialized", () => {
      getMyEnrollments.mockReturnValue(of(enrollments));

      createComponent();

      expect(getMyEnrollments).toHaveBeenCalledExactlyOnceWith();;
    });

    it("should store the enrollments returned by the API", () => {
      getMyEnrollments.mockReturnValue(of(enrollments));

      createComponent();

      expect(component.enrollments()).toEqual(enrollments);
    });    
  });

  describe("successful retrieval", () => {
    it("should display active benefit details", () => {
      getMyEnrollments.mockReturnValue(of(enrollments));

      createComponent();

      const renderedText = fixture.nativeElement.textContent as string;

      expect(renderedText).toContain("Active Enrollments");
      expect(renderedText).toContain("Extended Health Coverage");
      expect(renderedText).toContain("Health");
      expect(renderedText).toContain("Active");
    });

    it("should display historical benefit details and its end date", () => {
      getMyEnrollments.mockReturnValue(of(enrollments));

      createComponent();

      const renderedText = fixture.nativeElement.textContent as string;

      expect(renderedText).toContain("Enrollment History");
      expect(renderedText).toContain("Dental Coverage");
      expect(renderedText).toContain("Dental");
      expect(renderedText).toContain("Historical");
      expect(renderedText).toContain("Mar 10, 2025");
      expect(renderedText).toContain("Feb 28, 2026");      
    });

    it("should not display historical enrollments in the active benefits section", () => {
        getMyEnrollments.mockReturnValue(of(enrollments));

        createComponent();

        const activeSection = fixture.nativeElement.querySelector("#active-enrollments-heading")?.parentElement as HTMLElement;

        expect(activeSection.textContent).toContain( "Extended Health Coverage");
        expect(activeSection.textContent).not.toContain("Dental Coverage");
    });  
    
    it("should not display active enrollments in the enrollment history section", () => {
        getMyEnrollments.mockReturnValue(of(enrollments));

        createComponent();

        const historySection = fixture.nativeElement.querySelector("#enrollment-history-heading")?.parentElement as HTMLElement;

        expect(historySection.textContent).toContain("Dental Coverage");
        expect(historySection.textContent).not.toContain("Extended Health Coverage");
    });   
    
    it("should display the enrollment date without shifting the calendar day", () => {
        getMyEnrollments.mockReturnValue(
            of([
                {
                    benefitPlanId: 1,
                    benefitPlan: "Extended Health Coverage",
                    benefitType: "Health",
                    enrollmentDate: "2026-01-15",
                    endDate: null,
                    isActive: true
                }
            ])
        );

        createComponent();

        const renderedText = fixture.nativeElement.textContent as string;

        expect(renderedText).toContain("Jan 15, 2026");
    });
    
    it("should display a fallback when a historical enrollment has no end date", () => {
        getMyEnrollments.mockReturnValue(
            of([
                {
                    benefitPlanId: 3,
                    benefitPlan: "Vision Coverage",
                    benefitType: "Vision",
                    enrollmentDate: "2025-06-01",
                    endDate: null,
                    isActive: false
                }
            ])
        );

        createComponent();

        const renderedText = fixture.nativeElement.textContent as string;

        expect(renderedText).toContain("Vision Coverage");
        expect(renderedText).toContain("Not available");
    });

    it("should display only the sections that contain enrollments", () => {
        getMyEnrollments.mockReturnValue(
            of([
                {
                    benefitPlanId: 1,
                    benefitPlan: "Extended Health Coverage",
                    benefitType: "Health",
                    enrollmentDate: "2026-01-15",
                    endDate: null,
                    isActive: true
                }
            ])
        );

        createComponent();

        const renderedText = fixture.nativeElement.textContent as string;

        expect(renderedText).toContain("Active Enrollments");
        expect(renderedText).not.toContain("Enrollment History");
    });    
  });

  describe("empty state", () => {
    it("should display an empty state when the employee has no enrollments", () => {
        getMyEnrollments.mockReturnValue(of([]));

        createComponent();

        const renderedText = fixture.nativeElement.textContent as string;

        expect(renderedText).toContain("No benefits found");
        expect(renderedText).toContain(
            "You don't currently have any benefit enrollments."
        );

        expect(renderedText).not.toContain("Active Enrollments");
        expect(renderedText).not.toContain("Enrollment History");
    });    
  });

  describe("error handling", () => {
    it("should display an error state when retrieving enrollments fails", () => {
        getMyEnrollments.mockReturnValue(throwError(() => new Error("Request failed")));

        createComponent();

        const renderedText = fixture.nativeElement.textContent as string;

        expect(component.hasError()).toBe(true);
        expect(component.enrollments()).toEqual([]);

        expect(renderedText).toContain("Unable to load your benefit enrollments");
        expect(renderedText).toContain("Please try again");
    });

    it("should retry the request and display benefits after a previous failure", () => {
        getMyEnrollments.mockReturnValueOnce(throwError(() => new Error("Request failed")))
            .mockReturnValueOnce(of(enrollments));

        createComponent();

        expect(component.hasError()).toBe(true);

        const retryButton = fixture.nativeElement.querySelector("button") as HTMLButtonElement;

        retryButton.click();
        fixture.detectChanges();

        expect(getMyEnrollments).toHaveBeenCalledTimes(2);
        expect(component.hasError()).toBe(false);
        expect(component.enrollments()).toEqual(enrollments);

        const renderedText = fixture.nativeElement.textContent as string;

        expect(renderedText).toContain("Extended Health Coverage");
        expect(renderedText).toContain("Dental Coverage");
        expect(renderedText).not.toContain("Unable to load your benefits");
    });    
  });

  describe("loading state", () => {
    it("should show a loading indicator while the request is pending", () => {
        const pendingResponse$ = new Subject<EmployeeBenefitDto[]>();

        getMyEnrollments.mockReturnValue(pendingResponse$ as Observable<EmployeeBenefitDto[]>);

        createComponent();

        expect(component.isLoading()).toBe(true);

        expect(fixture.debugElement.query(By.directive(MatProgressSpinner))).not.toBeNull();
        expect(fixture.nativeElement.textContent).toContain("Loading your enrollments");

        pendingResponse$.next(enrollments);
        pendingResponse$.complete();

        fixture.detectChanges();

        expect(component.isLoading()).toBe(false);
        expect(component.enrollments()).toEqual(enrollments);
    });

    it("should stop loading when the request fails", () => {
        getMyEnrollments.mockReturnValue(throwError(() => new Error("Request failed")));

        createComponent();

        expect(component.isLoading()).toBe(false);
        expect(component.hasError()).toBe(true);
    });    
  });
});
