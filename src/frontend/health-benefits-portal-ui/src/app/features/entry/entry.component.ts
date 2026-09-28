import { Component, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '@core/auth/application/auth.service';
import { HomeComponent } from '../pages/home/home.component';

@Component({
  imports: [
    CommonModule,
    HomeComponent
  ],
  selector: 'app-entry',
  styleUrl: './entry.component.scss',
  templateUrl: './entry.component.html',
})
export class EntryComponent {
  private readonly authService = inject(AuthService);

  readonly isAuthenticated = this.authService.isAuthenticated;
}
