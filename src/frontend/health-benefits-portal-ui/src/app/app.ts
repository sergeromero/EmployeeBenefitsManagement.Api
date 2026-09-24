import { Component, signal } from '@angular/core';
import { AppNotification } from './core/notifications/notification.component';
import { RouterOutlet } from '@angular/router';

@Component({
  standalone: true,
  imports: [RouterOutlet, AppNotification],
  selector: 'app-root',
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('health-benefits-portal-ui');
}
