import { DatePipe } from "@angular/common";
import { Component, inject } from "@angular/core";
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from "@angular/forms";
import { MatButtonModule } from "@angular/material/button";
import { MatDatepickerModule } from "@angular/material/datepicker";
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from "@angular/material/dialog";
import { MatFormFieldModule } from "@angular/material/form-field";
import { MatInputModule } from "@angular/material/input";
import { provideNativeDateAdapter } from "@angular/material/core";
import { TerminateEnrollmentRequest } from "@core/contracts/enrollment/terminate-enrollment.request";
import { EmployeeBenefitDto } from "@core/contracts/employee/employee-benefit.dto";

@Component({
  imports: [
    DatePipe,
    ReactiveFormsModule,
    MatButtonModule,
    MatDatepickerModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule
  ],
  providers: [provideNativeDateAdapter()],
  selector: "app-terminate-enrollment-dialog",
  styleUrl: "./terminate-enrollment-dialog.component.scss",
  templateUrl: "./terminate-enrollment-dialog.component.html",
})
export class TerminateEnrollmentDialogComponent {
  private readonly dialogRef = inject<MatDialogRef<TerminateEnrollmentDialogComponent, TerminateEnrollmentRequest | null>>(MatDialogRef);
  enrollment = inject<EmployeeBenefitDto>(MAT_DIALOG_DATA);
  readonly minDate = this.getMinimumEndDate();

  readonly form = new FormGroup({
    endDate: new FormControl<Date | null>(new Date(), [
      Validators.required,
      control => {
        const value = control.value as Date | null;

        if(!value) {
          return null;
        }

        return this.toDateOnly(value) < this.enrollment.enrollmentDate
          ? { beforeEnrollmentDate: true }
          : null;
      }
    ])
  });

  cancel(): void {
    this.dialogRef.close(null);
  }

  confirm(): void {
    if(this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const endDate = this.form.controls.endDate.value;

    if(!endDate) {
      return;
    }

    const request: TerminateEnrollmentRequest = {
      benefitPlanId: this.enrollment.benefitPlanId,
      endDate: this.toDateOnly(endDate)
    };

    this.dialogRef.close(request);
  }

  private toDateOnly(value: Date): string {
      const year = value.getFullYear();
      const month = String(value.getMonth() + 1).padStart(2, "0");
      const day = String(value.getDate()).padStart(2, "0");

      return `${year}-${month}-${day}`;
  }  

  private getMinimumEndDate(): Date {
    const now = new Date();

    // Represent today's UTC calendar date as a local Date object.
    const today = new Date(
        now.getUTCFullYear(),
        now.getUTCMonth(),
        now.getUTCDate()
    );

    return today;
  }  
}
