import { Component, inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule, MatDialogRef, MAT_DIALOG_DATA } from "@angular/material/dialog"
import { BenefitPlanListItemDto } from '@core/contracts/benefit-plan/benefit-plan-list-item.dto';

@Component({
  imports: [
    MatButtonModule,
    MatDialogModule
  ],
  selector: 'app-confirmation-dialog',
  styleUrl: './confirmation-dialog.component.scss',
  templateUrl: './confirmation-dialog.component.html',
})
export class ConfirmationDialogComponent {
  private readonly dialogRef = inject<MatDialogRef<ConfirmationDialogComponent, boolean>>(MatDialogRef);
  readonly plan = inject<BenefitPlanListItemDto>(MAT_DIALOG_DATA);

  confirm(): void {
    this.dialogRef.close(true);
  }

  cancel(): void {
    this.dialogRef.close(false);
  }
}
