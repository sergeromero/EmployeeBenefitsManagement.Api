import { Component, input, computed, output } from '@angular/core';
import { NgClass } from '@angular/common';

@Component({
  imports: [NgClass],
  selector: 'app-button',
  styleUrls: ['./button.component.scss'],
  templateUrl: './button.component.html',
})
export class AppButton {
  variant = input<'primary' | 'secondary' | 'outline'>('primary');
  disabled = input<boolean>(false);

  classes = computed(() => ({
    'btn': true,
    'btn-primary': this.variant() === 'primary',
    'btn-secondary': this.variant() === 'secondary',
    'btn-outline': this.variant() === 'outline',
    'btn-disabled': this.disabled()
  }));

  clicked = output<void>();

  onClick(): void {
    if (!this.disabled()) {
      this.clicked.emit();
    }
  }
}
