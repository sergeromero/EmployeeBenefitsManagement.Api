import { Component, inject } from '@angular/core';
import { AppHeader } from '../header/header.component';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { AuthService } from '@core/auth/application/auth.service';

@Component({
  standalone: true,
  imports: [AppHeader, RouterOutlet, SidebarComponent],
  selector: 'app-main-layout',
  styleUrl: './main-layout.component.scss',
  templateUrl: './main-layout.component.html',
})
export class AppMainLayout {
  private readonly authService = inject(AuthService);

  readonly isAuthenticated = this.authService.isAuthenticated;
}
