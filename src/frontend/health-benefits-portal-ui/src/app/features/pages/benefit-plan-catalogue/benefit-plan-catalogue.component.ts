import { Component, DestroyRef, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog } from '@angular/material/dialog';
import { catchError, EMPTY, finalize, tap } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop'
import { BenefitPlansApiService } from '@core/api/benefit-plans/benefit-plans-api.service';
import { BenefitPlanListItemDto } from '@core/contracts/benefit-plan/benefit-plan-list-item.dto';
import { ConfirmationDialogComponent } from '../shared/confirmation-dialog/confirmation-dialog.component';
import { CreateEnrollmentRequest } from '@core/contracts/enrollment/create-enrollment.request';
import { EmployeeEnrollmentsApiService } from '@core/api/enrollments/employee-enrollments-api.service';
import { NotificationService } from '@core/notifications/notification.service';
import { Router } from '@angular/router';

@Component({
  imports: [
    MatButtonModule,
    MatCardModule,
    MatProgressSpinnerModule
  ],
  selector: 'app-benefit-plan-catalogue',
  styleUrl: './benefit-plan-catalogue.component.scss',
  templateUrl: './benefit-plan-catalogue.component.html',
})
export class BenefitPlanCatalogueComponent {
  private readonly benefitsApi = inject(BenefitPlansApiService);
  private readonly enrollmentsApi = inject(EmployeeEnrollmentsApiService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly dialog = inject(MatDialog);
  private readonly notificationService = inject(NotificationService);
  private readonly router = inject(Router);

  readonly benefitPlans = signal<BenefitPlanListItemDto[]>([]);
  readonly isLoading = signal(false);
  readonly hasError = signal(false);
  readonly isSubmitting = signal(false);
  readonly submittingPlanId = signal<number | null>(null);

  constructor() {
    this.loadBenefitPlans();
  }

  loadBenefitPlans(): void {
    this.isLoading.set(true);
    this.hasError.set(false);

    this.benefitsApi.getAll().pipe(
      catchError(() => {
        this.benefitPlans.set([]);
        this.hasError.set(true);

        return EMPTY;
      }),
      finalize(() => this.isLoading.set(false)),
      takeUntilDestroyed(this.destroyRef)
    ).subscribe(benefitPlans => {
      this.benefitPlans.set(benefitPlans)
    });
  }

  enroll(plan: BenefitPlanListItemDto): void {
    if (this.isSubmitting()) {
      return;
    }

    const dialogRef = this.dialog.open(ConfirmationDialogComponent, {
      data: plan
    });

    dialogRef.afterClosed()
    .pipe(takeUntilDestroyed(this.destroyRef))
    .subscribe(confirmed => {
      if (confirmed !== true || this.isSubmitting()) {
        return;
      }

      this.submitEnrollment(plan);
    });
  }

  private submitEnrollment(plan: BenefitPlanListItemDto): void {
    if(this.isSubmitting()) {
      return;
    }

    const request: CreateEnrollmentRequest = {
      benefitPlanId: plan.id,
      enrollmentDate: new Date().toISOString().slice(0, 10)
    };

    this.isSubmitting.set(true);
    this.submittingPlanId.set(plan.id);

    this.enrollmentsApi.create(request)
    .pipe(tap(() => {
      this.notificationService.showSuccess(`You have successfully enrolled in ${plan.name}.`);
      this.router.navigateByUrl("employees/my/enrollments");
    }),
    catchError(() => EMPTY),
    finalize(() => {
      this.isSubmitting.set(false);
      this.submittingPlanId.set(null);
    }),
    takeUntilDestroyed(this.destroyRef))
    .subscribe();
  }
}
