import { Component, inject } from '@angular/core';
import { AuthService } from '@core/auth/application/auth.service';

@Component({
  imports: [],
  selector: 'app-header',
  styleUrl: './header.component.scss',
  templateUrl: './header.component.html',
})
export class Header {
  private readonly authService = inject(AuthService);

  logout(): void {
    this.authService.logout();
  }
}
