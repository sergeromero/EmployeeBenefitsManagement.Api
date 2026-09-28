import { Component, computed, inject } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '@core/auth/application/auth.service';
import { LANDING_ACTIONS } from '../../features/pages/shared/landing.config';

@Component({
  imports: [RouterLink, RouterLinkActive],
  selector: 'app-sidebar',
  styleUrl: './sidebar.component.scss',
  templateUrl: './sidebar.component.html',
})
export class SidebarComponent {
  private readonly authService = inject(AuthService);

  readonly roles = this.authService.roles;

  readonly actions = computed(() => 
    LANDING_ACTIONS.filter(action => 
      action.roles.some(role => this.roles().includes(role))
    ));
}
