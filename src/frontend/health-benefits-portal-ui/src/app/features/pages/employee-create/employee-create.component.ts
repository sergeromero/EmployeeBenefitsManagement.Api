import { Component, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { EmployeesApiService } from '@core/api/employees/employees-api.service';
import { EmployeeFormComponent } from '../../../shared/components/employee-form/employee-form.component';
import { parseDateOnly } from '@core/utils/date.utils';
import { RoleDto } from '@core/contracts/role/role.dto';
import { AuthService } from '@core/auth/application/auth.service';

@Component({
  imports: [EmployeeFormComponent],
  selector: 'app-employee-create',
  styleUrl: './employee-create.component.scss',
  templateUrl: './employee-create.component.html',
})
export class EmployeeCreateComponent {
  private employeeApi = inject(EmployeesApiService);
  private router = inject(Router);
  private authService = inject(AuthService);

  roles = signal<RoleDto[]>([]);
  isAdmin = computed(() => this.authService.hasRole("Administrator"));

  constructor() {
    this.employeeApi.getRoles().subscribe(r => this.roles.set(r));
  }

  submit(form: any) {
    const raw = form.getRawValue();

    if(this.isAdmin()) {
      const request = {
        employee: {
          ...raw.employee,
          hireDate: parseDateOnly(raw.employee.hireDate)
        },
        user: raw.user,
        role: raw.role
      };

      this.employeeApi.createWithUser(request).subscribe(() => {
        this.router.navigate(["/employees"]);
      });
    } else {
      const request = {
        employeeNumber: raw.employee.employeeNumber,
        firstName: raw.employee.firstName,
        lastName: raw.employee.lastName,
        email: raw.employee.email,
        hireDate: parseDateOnly(raw.employee.hireDate),
        departmentId: raw.employee.departmentId
      };  
      
      this.employeeApi.create(request).subscribe(() => {
        this.router.navigate(["/employees"]);
      })
    }
  }
}
