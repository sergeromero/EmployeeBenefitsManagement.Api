import { Component, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '@core/auth/application/auth.service';
import { LANDING_ACTIONS } from '../shared/landing.config';
import { LandingAction, UserRole } from '../shared/landing.model';

@Component({
  imports: [CommonModule, RouterModule],
  selector: 'app-admin',
  styleUrl: './admin.component.scss',
  templateUrl: './admin.component.html',
})
export class AdminComponent {
  private authService = inject(AuthService);
  private roles =  computed<UserRole[]>(() => (this.authService.roles() ?? []) as UserRole[]);

  actions = computed<LandingAction[]>(() => 
    LANDING_ACTIONS.filter(action => 
      action.roles.some(role => this.roles().includes(role))
    ));
}
