import { Component, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '@core/auth/application/auth.service';
import { HomeComponent } from '../pages/home/home.component';
import { AdminComponent } from '../pages/admin/admin.component';
import { EmployeeComponent } from '../pages/employee/employee.component';

@Component({
  imports: [
    CommonModule,
    HomeComponent,
    AdminComponent,
    EmployeeComponent
  ],
  selector: 'app-entry',
  styleUrl: './entry.component.scss',
  templateUrl: './entry.component.html',
})
export class EntryComponent {
  private authService = inject(AuthService);
  private user = this.authService.user;

  view = computed<"home" | "admin" | "employee">(() => {
    const user = this.user();

    if(!user) {
      return "home";
    }

    if (user.roles.includes("Administrator")) {
      return "admin";
    }

    return "employee";
  })
}
