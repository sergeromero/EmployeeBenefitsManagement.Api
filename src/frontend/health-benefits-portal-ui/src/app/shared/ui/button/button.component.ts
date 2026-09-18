import { Component, input, computed } from '@angular/core';
import { NgClass } from '@angular/common';

@Component({
  imports: [NgClass],
  selector: 'app-button',
  styleUrl: './button.component.scss',
  templateUrl: './button.component.html',
})
export class AppButton {
  label = input<string>('');
  variant = input<'primary' | 'secondary' | 'outline'>('primary');
  disabled = input<boolean>(false);

  classes = computed(() => ({
    'btn': true,
    'btn-primary': this.variant() === 'primary',
    'btn-secondary': this.variant() === 'secondary',
    'btn-outline': this.variant() === 'outline',
    'btn-disabled': this.disabled()
  }));
}
