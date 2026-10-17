import { Component, DestroyRef, inject, signal } from '@angular/core';
import { DatePipe } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { catchError, EMPTY, finalize } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { EmployeeEnrollmentsApiService } from '@core/api/enrollments/employee-enrollments-api.service';
import { EmployeeBenefitDto } from '@core/contracts/employee/employee-benefit.dto';

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
}
