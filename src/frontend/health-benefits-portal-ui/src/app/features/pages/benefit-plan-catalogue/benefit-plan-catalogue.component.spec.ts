import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Observable, of, Subject, throwError } from 'rxjs';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { MatDialog } from '@angular/material/dialog';
import { By } from '@angular/platform-browser';
import { Mock } from 'vitest';
import { BenefitPlanCatalogueComponent } from './benefit-plan-catalogue.component';
import { BenefitPlansApiService } from '@core/api/benefit-plans/benefit-plans-api.service';
import { BenefitPlanListItemDto } from '@core/contracts/benefit-plan/benefit-plan-list-item.dto';
import { ConfirmationDialogComponent } from '../shared/confirmation-dialog/confirmation-dialog.component';
import { EmployeeEnrollmentsApiService } from '@core/api/enrollments/employee-enrollments-api.service';
import { NotificationService } from '@core/notifications/notification.service';
import { Router } from '@angular/router';
import { CreateEnrollmentRequest } from '@core/contracts/enrollment/create-enrollment.request';

describe('BenefitPlanCatalogueComponent', () => {
  let component: BenefitPlanCatalogueComponent;
  let fixture: ComponentFixture<BenefitPlanCatalogueComponent>;
  let getAll: Mock<BenefitPlansApiService["getAll"]>;
  let openDialog: Mock<
    (
      component: typeof ConfirmationDialogComponent,
      config: { data: BenefitPlanListItemDto }
    ) => {
      afterClosed: () => Observable<boolean | undefined>;
    }
  >;

  let createEnrollment: Mock<EmployeeEnrollmentsApiService["create"]>;
  let afterClosed$: Subject<boolean | undefined>;

  let showSuccess: Mock<NotificationService["showSuccess"]>;
  let navigateByUrl: Mock<Router["navigateByUrl"]>;

  const plans: BenefitPlanListItemDto[] = [
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
    },
    {
      id: 2,
      name: "Dental Coverage",
      description: "Coverage for eligible dental expenses.",
      benefitType: {
          id: 2,
          name: "Dental"
      },
      enrollmentCategoryName: "Health Benefits",
      enrollmentEligibility: {
        canEnroll: true,
        reason: null
      }
    }    
  ];

  function createComponent(): void {
    fixture = TestBed.createComponent(BenefitPlanCatalogueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }

  beforeEach(async () => {
    getAll = vi.fn<BenefitPlansApiService["getAll"]>();
    afterClosed$ = new Subject<boolean | undefined>;
    createEnrollment = vi.fn<EmployeeEnrollmentsApiService["create"]>();
    openDialog = vi.fn<
    (
      component: typeof ConfirmationDialogComponent,
      config: { data: BenefitPlanListItemDto }
    ) => {
          afterClosed: () => Observable<boolean | undefined>;
        }
    >(() => ({
      afterClosed: () => afterClosed$.asObservable()
    }));

    showSuccess = vi.fn<NotificationService["showSuccess"]>();
    navigateByUrl = vi.fn<Router["navigateByUrl"]>();

    await TestBed.configureTestingModule({
      imports: [BenefitPlanCatalogueComponent],
      providers: [
        {
          provide: BenefitPlansApiService,
          useValue: {
            getAll
          }
        },
        {
          provide: MatDialog,
          useValue: {
            open: openDialog
          }
        },
        {
          provide: EmployeeEnrollmentsApiService,
          useValue: {
            create: createEnrollment
          }
        },
        {
          provide: NotificationService,
          useValue: {
            showSuccess
          }
        },
        {
          provide: Router,
          useValue: {
            navigateByUrl
          }
        }
      ]
    });    
  });

  it('should create', () => {
    getAll.mockReturnValue(of());
    createComponent();
    expect(component).toBeTruthy();
  });

  it("should request benefit plans when initialized", () => {
    getAll.mockReturnValue(of(plans));

    createComponent();

    expect(getAll).toHaveBeenCalledOnce();
  });

  it("should display the details of the returned benefit plans", () => {
    getAll.mockReturnValue(of(plans));

    createComponent();

    const renderedText = fixture.nativeElement.textContent as string;

    expect(renderedText).toContain("Extended Health Coverage");
    expect(renderedText).toContain("Coverage for eligible healthcare expenses.");
    expect(renderedText).toContain("Health");
    expect(renderedText).toContain("Health Benefits");

    expect(renderedText).toContain("Dental Coverage");
    expect(renderedText).toContain("Coverage for eligible dental expenses.");
    expect(renderedText).toContain("Dental");    
  });

  it("should display an error state when loading benefit plan fails", () => {
    getAll.mockReturnValue(throwError(() => new Error("Any error")));

    createComponent();

    const renderedText = fixture.nativeElement.textContent as string;

    expect(renderedText).toContain("Unable to load benefit plans");
    expect(renderedText).toContain("Please try again");
  });

  it("should retry the request and display plans after a previous failure", () => {
    getAll.mockReturnValueOnce(throwError(() => new Error("Any error")))
      .mockReturnValueOnce(of(plans));

    createComponent();

    expect(component.hasError()).toBe(true);

    const retryButton = fixture.nativeElement.querySelector("button") as HTMLButtonElement;

    retryButton.click();
    fixture.detectChanges();

    expect(getAll).toHaveBeenCalledTimes(2);
    expect(component.hasError()).toBe(false);
    expect(component.benefitPlans()).toEqual(plans);
    expect(fixture.nativeElement.textContent).toContain("Extended Health Coverage");
  });

  it("should show a loading indicator while the request is pending", () => {
    const pendingResponse$ = new Subject<BenefitPlanListItemDto[]>();

    getAll.mockReturnValue(pendingResponse$ as Observable<BenefitPlanListItemDto[]>);

    createComponent();

    expect(component.isLoading()).toBe(true);
    expect(fixture.debugElement.query(By.directive(MatProgressSpinner))).not.toBeNull();
    
    pendingResponse$.next(plans);
    pendingResponse$.complete();

    fixture.detectChanges();

    expect(component.isLoading()).toBe(false);
  });

  it("should open the confirmation dialog with the selected benefit plan", () => {
    getAll.mockReturnValue(of(plans));

    createComponent();

    const enrollButton = fixture.nativeElement.querySelectorAll(".benefit-plan-card__actions button") as NodeListOf<HTMLButtonElement>;

    enrollButton[0].click();

    expect(openDialog).toHaveBeenCalledExactlyOnceWith(ConfirmationDialogComponent, { data: plans[0]});
  });

  it('should not submit an enrollment when the dialog is cancelled', () => {
    getAll.mockReturnValue(of(plans));
    createEnrollment.mockReturnValue(of(void 0));

    createComponent();

    const enrollButton = fixture.nativeElement.querySelector(
      '.benefit-plan-card__actions button'
    ) as HTMLButtonElement;

    enrollButton.click();
    afterClosed$.next(false);
    afterClosed$.complete();

    expect(createEnrollment).not.toHaveBeenCalled();
  });  

  it('should not submit an enrollment when the confirmation dialog is cancelled', () => {
    getAll.mockReturnValue(of(plans));
    createEnrollment.mockReturnValue(of(void 0));

    createComponent();

    component.enroll(plans[0]);
    afterClosed$.next(false);
    afterClosed$.complete();

    expect(createEnrollment).not.toHaveBeenCalled();
    expect(showSuccess).not.toHaveBeenCalled();
    expect(navigateByUrl).not.toHaveBeenCalled();
  });

  it('should submit the selected plan with the expected request payload', () => {
    getAll.mockReturnValue(of(plans));
    createEnrollment.mockReturnValue(of(void 0));

    createComponent();

    component.enroll(plans[0]);
    afterClosed$.next(true);
    afterClosed$.complete();

    expect(createEnrollment).toHaveBeenCalledOnce();

    const request = createEnrollment.mock.calls[0][0] as CreateEnrollmentRequest;

    expect(request.benefitPlanId).toBe(plans[0].id);
    expect(request.enrollmentDate).toMatch(/^\d{4}-\d{2}-\d{2}$/);
  });

  it('should show the pending state while enrollment submission is in progress', () => {
    const pendingResponse$ = new Subject<void>();

    getAll.mockReturnValue(of(plans));
    createEnrollment.mockReturnValue(pendingResponse$);

    createComponent();

    component.enroll(plans[0]);
    afterClosed$.next(true);
    afterClosed$.complete();

    fixture.detectChanges();

    expect(component.isSubmitting()).toBe(true);
    expect(component.submittingPlanId()).toBe(plans[0].id);

    const buttons = Array.from(
      fixture.nativeElement.querySelectorAll(
        '.benefit-plan-card__actions button'
      ) as NodeListOf<HTMLButtonElement>
    );

    expect(buttons).toHaveLength(plans.length);
    expect(buttons.every(button => button.disabled)).toBe(true);
    expect(buttons[0].textContent).toContain('Submitting ...');
    expect(buttons[1].textContent).toContain('Enroll');

    pendingResponse$.next();
    pendingResponse$.complete();

    fixture.detectChanges();

    expect(component.isSubmitting()).toBe(false);
    expect(component.submittingPlanId()).toBeNull();
    expect(buttons.every(button => button.disabled)).toBe(false);
  });

  it('should show a success notification and navigate to My Enrollments after submission succeeds', () => {
    getAll.mockReturnValue(of(plans));
    createEnrollment.mockReturnValue(of(void 0));
    navigateByUrl.mockResolvedValue(true);

    createComponent();

    component.enroll(plans[0]);
    afterClosed$.next(true);
    afterClosed$.complete();

    expect(showSuccess).toHaveBeenCalledExactlyOnceWith(
      `You have successfully enrolled in ${plans[0].name}.`
    );

    expect(navigateByUrl).toHaveBeenCalledExactlyOnceWith(
      'employees/my/enrollments'
    );

    expect(component.isSubmitting()).toBe(false);
    expect(component.submittingPlanId()).toBeNull();
  });

  it('should reset the pending state without showing success or navigating when submission fails', () => {
    getAll.mockReturnValue(of(plans));
    createEnrollment.mockReturnValue(
      throwError(() => new Error('Enrollment failed'))
    );

    createComponent();

    component.enroll(plans[0]);
    afterClosed$.next(true);
    afterClosed$.complete();

    fixture.detectChanges();

    expect(showSuccess).not.toHaveBeenCalled();
    expect(navigateByUrl).not.toHaveBeenCalled();

    expect(component.isSubmitting()).toBe(false);
    expect(component.submittingPlanId()).toBeNull();

    const buttons = Array.from(
      fixture.nativeElement.querySelectorAll(
        '.benefit-plan-card__actions button'
      ) as NodeListOf<HTMLButtonElement>
    );

    expect(buttons.every(button => !button.disabled)).toBe(true);
  });  

  it("should allow enrollment when the plan is eligible", () => {
      getAll.mockReturnValue(of([
          {
              ...plans[0],
              enrollmentEligibility: {
                  canEnroll: true,
                  reason: null
              }
          }
      ]));

      createComponent();

      const card = fixture.nativeElement.querySelector("mat-card") as HTMLElement;
      const enrollButton = card.querySelector("button") as HTMLButtonElement;

      expect(card.classList.contains("benefit-plan-card--ineligible")).toBe(false);
      expect(card.textContent).not.toContain("Already enrolled in benefit");
      expect(card.textContent).not.toContain("Already enrolled in category");
      expect(enrollButton.disabled).toBe(false);
  });

  it("should indicate when the employee is already enrolled in the benefit", () => {
      getAll.mockReturnValue(of([
          {
              ...plans[0],
              enrollmentEligibility: {
                  canEnroll: false,
                  reason: "AlreadyEnrolledInBenefit"
              }
          }
      ]));

      createComponent();

      const card = fixture.nativeElement.querySelector("mat-card") as HTMLElement;
      const enrollButton = card.querySelector("button") as HTMLButtonElement;

      expect(card.classList.contains("benefit-plan-card--ineligible")).toBe(true);
      expect(card.textContent).toContain("You are already enrolled in this benefit");
      expect(enrollButton.disabled).toBe(true);
  });

  it("should indicate when the employee is already enrolled in the category", () => {
      getAll.mockReturnValue(of([
          {
              ...plans[0],
              enrollmentEligibility: {
                  canEnroll: false,
                  reason: "AlreadyEnrolledInCategory"
              }
          }
      ]));

      createComponent();

      const card = fixture.nativeElement.querySelector("mat-card") as HTMLElement;
      const enrollButton = card.querySelector("button") as HTMLButtonElement;

      expect(card.classList.contains("benefit-plan-card--ineligible")).toBe(true);
      expect(card.textContent).toContain("You are already enrolled in a benefit from this category");
      expect(enrollButton.disabled).toBe(true);
  });  

  it("should not open the confirmation dialog for an ineligible plan", () => {
      getAll.mockReturnValue(of([
          {
              ...plans[0],
              enrollmentEligibility: {
                  canEnroll: false,
                  reason: "AlreadyEnrolledInBenefit"
              }
          }
      ]));

      createComponent();

      component.enroll(component.benefitPlans()[0]);

      expect(openDialog).not.toHaveBeenCalled();
      expect(createEnrollment).not.toHaveBeenCalled();
  });  
});
