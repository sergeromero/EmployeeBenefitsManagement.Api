import { Component, DestroyRef, inject, signal } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { catchError, EMPTY, finalize } from 'rxjs';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop'
import { BenefitPlansApiService } from '@core/api/benefit-plans/benefit-plans-api.service';
import { BenefitPlanListItemDto } from '@core/contracts/benefit-plan/benefit-plan-list-item.dto';

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
  private readonly api = inject(BenefitPlansApiService);
  private readonly destroyRef = inject(DestroyRef);

  readonly benefitPlans = signal<BenefitPlanListItemDto[]>([]);
  readonly isLoading = signal(false);
  readonly hasError = signal(false);

  constructor() {
    this.loadBenefitPlans();
  }

  loadBenefitPlans(): void {
    this.isLoading.set(true);
    this.hasError.set(false);

    this.api.getAll().pipe(
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
}
