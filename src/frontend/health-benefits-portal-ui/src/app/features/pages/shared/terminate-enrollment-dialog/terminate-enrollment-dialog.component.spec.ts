import { ComponentFixture, TestBed } from "@angular/core/testing";
import { MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog";
import { Mock } from "vitest";

import { EmployeeBenefitDto } from "@core/contracts/employee/employee-benefit.dto";
import { TerminateEnrollmentRequest } from "@core/contracts/enrollment/terminate-enrollment.request";
import { TerminateEnrollmentDialogComponent } from "./terminate-enrollment-dialog.component";

describe('TerminateEnrollmentDialogComponent', () => {
  let component: TerminateEnrollmentDialogComponent;
  let fixture: ComponentFixture<TerminateEnrollmentDialogComponent>;

  let close: Mock<
      MatDialogRef<
          TerminateEnrollmentDialogComponent,
          TerminateEnrollmentRequest | null
      >["close"]
  >;

  const enrollment: EmployeeBenefitDto = {
      benefitPlanId: 5,
      benefitPlan: "Extended Health Coverage",
      benefitType: "Health",
      enrollmentDate: "2026-01-15",
      endDate: null,
      isActive: true
  };

  function createComponent(): void {
      fixture = TestBed.createComponent(TerminateEnrollmentDialogComponent);
      component = fixture.componentInstance;
      fixture.detectChanges();
  }

  beforeEach(async () => {
    close = vi.fn<MatDialogRef<
            TerminateEnrollmentDialogComponent,
            TerminateEnrollmentRequest | null>["close"]
    >();
       
    await TestBed.configureTestingModule({
        imports: [TerminateEnrollmentDialogComponent],
        providers: [
            {
                provide: MatDialogRef,
                useValue: { close }
            },
            {
                provide: MAT_DIALOG_DATA,
                useValue: enrollment
            }
        ]
    }).compileComponents();
  });

  describe("initialization", () => {
      it("should create", () => {
          createComponent();
          expect(component).toBeTruthy();
      });

      it("should initialize the end date", () => {
          createComponent();

          const endDate = component.form.controls.endDate.value;

          expect(endDate).toBeInstanceOf(Date);
          expect(endDate!.getFullYear()).toBe(new Date().getFullYear());
          expect(endDate!.getMonth()).toBe(new Date().getMonth());
          expect(endDate!.getDate()).toBe(new Date().getDate());
      });

      it("should set the minimum date to the enrollment date", () => {
          createComponent();

          const today = new Date();

          expect(component.minDate.getFullYear()).toBe(today.getFullYear());
          expect(component.minDate.getMonth()).toBe(today.getMonth());
          expect(component.minDate.getDate()).toBe(today.getDate());
      });

      it("should display the enrollment details", () => {
          createComponent();

          const renderedText = fixture.nativeElement.textContent as string;

          expect(renderedText).toContain("Terminate Benefit Enrollment");
          expect(renderedText).toContain(enrollment.benefitPlan);
          expect(renderedText).toContain(enrollment.benefitType);
          expect(renderedText).toContain("Jan 15, 2026");
          expect(renderedText).toContain("Last active day");
      });
  });

  describe("end date validation", () => {
      it("should reject an end date before the enrollment date", () => {
          createComponent();

          component.form.controls.endDate.setValue(new Date(2026, 0, 14));

          expect(component.form.controls.endDate.hasError("beforeEnrollmentDate")).toBe(true);
          expect(component.form.invalid).toBe(true);
      });

      it("should accept an end date equal to today", () => {
          createComponent();

          const today = new Date();

          component.form.controls.endDate.setValue(today);

          expect(component.form.controls.endDate.valid).toBe(true);
      });

      it("should accept an end date after today", () => {
          createComponent();

          const tomorrow = new Date();
          tomorrow.setDate(tomorrow.getDate() + 1);

          component.form.controls.endDate.setValue(tomorrow);

          expect(component.form.controls.endDate.valid).toBe(true);
      });

      it("should reject a missing end date", () => {
          createComponent();

          component.form.controls.endDate.setValue(null);

          expect(component.form.controls.endDate.hasError("required")).toBe(true);
          expect(component.form.invalid).toBe(true);
      });
  });

  describe("confirmation", () => {
      it("should close with the termination request when confirmed", () => {
          createComponent();

          component.form.controls.endDate.setValue(new Date(2026, 10, 30));

          component.confirm();

          const expectedRequest: TerminateEnrollmentRequest = {
              benefitPlanId: enrollment.benefitPlanId,
              endDate: "2026-11-30"
          };

          expect(close).toHaveBeenCalledExactlyOnceWith(expectedRequest);
      });

      it("should not close when the form is invalid", () => {
          createComponent();

          component.form.controls.endDate.setValue(new Date(2026, 0, 14));

          component.confirm();

          expect(close).not.toHaveBeenCalled();
      });

      it("should mark the form controls as touched when confirmation is attempted with invalid data", () => {
          createComponent();

          component.form.controls.endDate.setValue(null);

          component.confirm();

          expect(component.form.controls.endDate.touched).toBe(true);
      });
  });

  describe("cancellation", () => {
      it("should close with null when cancelled", () => {
          createComponent();

          component.cancel();

          expect(close).toHaveBeenCalledExactlyOnceWith(null);
      });
  });  
});
