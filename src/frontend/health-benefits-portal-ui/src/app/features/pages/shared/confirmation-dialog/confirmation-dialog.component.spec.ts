import { ComponentFixture, TestBed } from '@angular/core/testing';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Mock } from 'vitest';
import { BenefitPlanListItemDto } from '@core/contracts/benefit-plan/benefit-plan-list-item.dto';
import { ConfirmationDialogComponent } from './confirmation-dialog.component';

describe('ConfirmationDialogComponent', () => {
  let component: ConfirmationDialogComponent;
  let fixture: ComponentFixture<ConfirmationDialogComponent>;

  let close: Mock<MatDialogRef<ConfirmationDialogComponent, boolean>["close"]>;

  const plan: BenefitPlanListItemDto = {
    id: 1,
    name: 'Extended Health Coverage',
    description: 'Coverage for eligible healthcare expenses.',
    benefitType: {
      id: 1,
      name: 'Health'
    },
    enrollmentCategoryName: 'Health Benefits',
    enrollmentEligibility: {
      canEnroll: true,
      reason: null
    }
  };  

  function createComponent(): void {
    fixture = TestBed.createComponent(ConfirmationDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  }

  beforeEach(async () => {
    close = vi.fn<MatDialogRef<ConfirmationDialogComponent, boolean>["close"]>();

    await TestBed.configureTestingModule({
      imports: [ConfirmationDialogComponent],
      providers: [
        {
          provide: MatDialogRef,
          useValue: { close }
        },
        {
          provide: MAT_DIALOG_DATA,
          useValue: plan
        }
      ]
    }).compileComponents();
  });

  it('should create', () => {
    createComponent();
    expect(component).toBeTruthy();
  });

  it("should close the dialog with true when enrollment is confirmed", () => {
    createComponent();

    component.confirm();

    expect(close).toHaveBeenCalledExactlyOnceWith(true);
  });

  it("should close the dialog with false when enrollment is cancelled", () => {
    createComponent();

    component.cancel();

    expect(close).toHaveBeenCalledExactlyOnceWith(false);
  });

  it("should display the selected benefit plan details", () => {
    createComponent();

    const renderedText = fixture.nativeElement.textContent as string;

    expect(renderedText).toContain('Confirm Benefit Enrollment');
    expect(renderedText).toContain(plan.name);
    expect(renderedText).toContain(plan.description);
    expect(renderedText).toContain(plan.benefitType.name);
    expect(renderedText).toContain(plan.enrollmentCategoryName);
    expect(renderedText).toContain('Confirm enrollment');
    expect(renderedText).toContain('Cancel');
  });  
});
