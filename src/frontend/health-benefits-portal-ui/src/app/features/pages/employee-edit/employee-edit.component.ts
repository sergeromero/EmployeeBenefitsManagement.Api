import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EmployeesApiService } from '@core/api/employees/employees-api.service';
import { EmployeeFormComponent } from '../../../shared/components/employee-form/employee-form.component';
import { parseDateOnly } from '@core/utils/date.utils';
import { RoleDto } from '@core/contracts/role/role.dto';
import { AuthService } from '@core/auth/application/auth.service';
import { UpdateEmployeeRequest } from '@core/contracts/employee/update-employee.request';

@Component({
  imports: [EmployeeFormComponent],
  selector: 'app-employee-edit',
  styleUrl: './employee-edit.component.scss',
  templateUrl: './employee-edit.component.html',
})
export class EmployeeEditComponent {
  private employeeApi = inject(EmployeesApiService);
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  roles = signal<RoleDto[]>([]);
  data = signal<any | null>(null);
  isAdmin = computed(() => this.authService.hasRole("Administrator"));

  constructor() {
    const id = Number(this.route.snapshot.paramMap.get('id'));

    this.employeeApi.getRoles().subscribe(r => this.roles.set(r));

    if(this.isAdmin()) {
      this.employeeApi.getCompleteById(id).subscribe(d => {
        this.data.set(d);
      })
    } else {
      this.employeeApi.getById(id).subscribe(d => {
        this.data.set(d);
      });
    }
  }

  submit(form: any) {
    const raw = form.getRawValue();

    if(this.isAdmin()){
      const request = {
        id: raw.employee.id,
        employee: {
          ...raw.employee,
          hireDate: parseDateOnly(raw.employee.hireDate)
        },
        user: {
          ...raw.user,
          password: raw.user.password || ""
        },
        role: raw.role
      };

      this.employeeApi.updateWithUser(request).subscribe(() => {
        this.router.navigate(['/employees']);
      });
    } else {
      const request: UpdateEmployeeRequest = {
        id: raw.employee.id,
        employeeNumber: raw.employee.employeeNumber,
        firstName: raw.employee.firstName,
        lastName: raw.employee.lastName,
        email: raw.employee.email,
        hireDate: parseDateOnly(raw.employee.hireDate),
        departmentId: raw.employee.departmentId
      };

      this.employeeApi.update(request).subscribe(() => {
        this.router.navigate(["/employees"]);
      })
    }
  }  
}
