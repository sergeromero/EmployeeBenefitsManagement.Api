import { Component, DestroyRef, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog } from '@angular/material/dialog';
import { catchError, EMPTY, finalize, switchMap, tap } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { EmployeeEnrollmentsApiService } from '@core/api/enrollments/employee-enrollments-api.service';
import { EmployeeBenefitDto } from '@core/contracts/employee/employee-benefit.dto';
import { NotificationService } from '@core/notifications/notification.service';
import { TerminateEnrollmentDialogComponent } from '../shared/terminate-enrollment-dialog/terminate-enrollment-dialog.component';
import { TerminateEnrollmentRequest } from '@core/contracts/enrollment/terminate-enrollment.request';

@Component({
  imports: [
    DatePipe,
    MatButtonModule,
    MatCardModule,
    MatProgressSpinnerModule
  ],
  selector: 'app-my-enrollments.component.ts',
  styleUrl: './my-enrollments.component.scss',
  templateUrl: './my-enrollments.component.html',
})
export class MyEnrollmentsComponent {
  private readonly api = inject(EmployeeEnrollmentsApiService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly dialog = inject(MatDialog);
  private readonly notifications = inject(NotificationService);

  readonly enrollments = signal<EmployeeBenefitDto[]>([]);
  readonly isLoading = signal(false);
  readonly hasError = signal(false);

  readonly activeEnrollments = () => this.enrollments().filter(e => e.isActive);
  readonly enrollmentHistory = () => this.enrollments().filter(e => !e.isActive);

  constructor() {
    this.loadBenefits();
  }

  loadBenefits(): void {
    this.isLoading.set(true);
    this.hasError.set(false);

    this.api.getMyEnrollments().pipe(
      catchError(() => {
        this.enrollments.set([]);
        this.hasError.set(true);
        return EMPTY;
      }),
      finalize(() => this.isLoading.set(false)),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(enrollments => {
      this.enrollments.set(enrollments);
    });
  }

  terminateEnrollment(enrollment: EmployeeBenefitDto): void {
    const dialogRef = this.dialog.open<
      TerminateEnrollmentDialogComponent,
      EmployeeBenefitDto,
      TerminateEnrollmentRequest | null>(TerminateEnrollmentDialogComponent, {
      data: enrollment
    });

    dialogRef.afterClosed().pipe(
      switchMap(request => {
        if (!request) {
          return EMPTY;
        }

        return this.api.terminate(request).pipe(
          tap(() => {
            this.notifications.showSuccess("Benefit enrollment termination scheduled successfully.");
            this.loadBenefits();
          })
        );
      }),
      catchError(() => EMPTY),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe();
  }  
}
