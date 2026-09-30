import { Component, computed, inject, signal } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AuthService } from '@core/auth/application/auth.service';
import { LANDING_ACTIONS } from '../../features/pages/shared/landing.config';
import { LandingAction } from '../../features/pages/shared/landing.model';

@Component({
  imports: [RouterModule],
  selector: 'app-sidebar',
  styleUrl: './sidebar.component.scss',
  templateUrl: './sidebar.component.html',
})
export class SidebarComponent {
  private readonly authService = inject(AuthService);

  expandedSections = signal<Record<string, boolean>>({});

  actions = computed(() => {
    return LANDING_ACTIONS.filter(a => this.hasAccess(a))
      .map(a => ({...a, children: a.children?.filter(c => this.hasAccess(c))
      }));
  });

  toggleSection(id: string) {
    this.expandedSections.update(state => ({
      ...state, [id]: !state[id]
    }));
  }

  isExpanded(id: string): boolean {
    return !!this.expandedSections()[id];
  }

  private hasAccess(action: LandingAction): boolean {
    return action.roles.some(role => this.authService.hasRole(role));
  }
}
