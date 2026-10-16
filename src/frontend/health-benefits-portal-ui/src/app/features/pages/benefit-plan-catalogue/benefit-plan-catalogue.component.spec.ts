import { ComponentFixture, TestBed } from '@angular/core/testing';
import { BenefitPlanCatalogueComponent } from './benefit-plan-catalogue.component';
import { Mock } from 'vitest';
import { BenefitPlansApiService } from '@core/api/benefit-plans/benefit-plans-api.service';
import { BenefitPlanListItemDto } from '@core/contracts/benefit-plan/benefit-plan-list-item.dto';
import { Observable, of, Subject, throwError } from 'rxjs';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { By } from '@angular/platform-browser';

describe('BenefitPlanCatalogueComponent', () => {
  let component: BenefitPlanCatalogueComponent;
  let fixture: ComponentFixture<BenefitPlanCatalogueComponent>;
  let getAll: Mock<BenefitPlansApiService["getAll"]>;

  const plans: BenefitPlanListItemDto[] = [
    {
      id: 1,
      name: "Extended Health Coverage",
      description: "Coverage for eligible healthcare expenses.",
      benefitType: {
          id: 1,
          name: "Health"
      },
      enrollmentCategoryName: "Health Benefits"
    },
    {
      id: 2,
      name: "Dental Coverage",
      description: "Coverage for eligible dental expenses.",
      benefitType: {
          id: 2,
          name: "Dental"
      },
      enrollmentCategoryName: "Health Benefits"
    }    
  ];

  function createComponent(): void {
    fixture = TestBed.createComponent(BenefitPlanCatalogueComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }

  beforeEach(async () => {
    getAll = vi.fn<BenefitPlansApiService["getAll"]>();

    await TestBed.configureTestingModule({
      imports: [BenefitPlanCatalogueComponent],
      providers: [
        {
          provide: BenefitPlansApiService,
          useValue: {
            getAll
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
  })
});
