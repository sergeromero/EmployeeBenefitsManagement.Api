import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { MainLayout } from './layout/main-layout/main-layout.component';
import { AppNotification } from './core/notifications/notification.component';

@Component({
  standalone: true,
  imports: [RouterOutlet, MainLayout, AppNotification],
  selector: 'app-root',
  styleUrl: './app.scss',
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('health-benefits-portal-ui');
}
