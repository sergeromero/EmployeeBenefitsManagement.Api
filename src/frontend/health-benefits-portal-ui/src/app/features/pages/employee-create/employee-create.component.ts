import { Component, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatDividerModule } from '@angular/material/divider';
import { MatCardModule } from '@angular/material/card';
import { Router } from '@angular/router';
import { EmployeesApiService } from '@core/api/employees/employees-api.service';
import { AuthService } from '@core/auth/application/auth.service';
import { RoleDto } from '@core/contracts/role/role.dto';
import { CreateEmployeeWithUserRequest } from '@core/contracts/employee/create-employee-with-user.request';
import { CreateEmployeeRequest } from '@core/contracts/employee/create-employee.request';
import { parseDateOnly } from '@core/utils/date.utils';

@Component({
  imports: [
    CommonModule, 
    ReactiveFormsModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatDividerModule,
    MatCardModule
  ],
  selector: 'app-employee-create',
  styleUrl: './employee-create.component.scss',
  templateUrl: './employee-create.component.html',
})
export class EmployeeCreateComponent {
  private formBuilder = inject(FormBuilder);
  private api = inject(EmployeesApiService);
  private auth = inject(AuthService);
  private router = inject(Router);

  roles = signal<RoleDto[]>([]);
  loading = signal(false);

  isAdmin = computed(() => this.auth.hasRole("Administrator"));

  form = this.formBuilder.group({
    employee: this.formBuilder.group({
      employeeNumber: ['', Validators.required],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      hireDate: ['', Validators.required],
      departmentId: [0, Validators.required],
    }),
    user: this.formBuilder.group({
      userName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required],
    }),
    role: ['', Validators.required]
  });

  constructor() {
    this.loadRoles();
    this.configureFormByRole();
  }

  private loadRoles() {
    this.api.getRoles().subscribe(r => this.roles.set(r))
  }

  private configureFormByRole() {
    if (!this.isAdmin()) {
      this.form.get('user')?.disable();
      this.form.get('role')?.disable();
    }
  }
  submit() {
    if(this.form.invalid) {
      return;
    }

    this.loading.set(true);

    const values = this.form.getRawValue();

    if (this.isAdmin()) {
      const request = this.mapToCreateEmployeeWithUserRequest();

      this.api.createWithUser(request).subscribe({
        next: () => this.onSuccess(),
        error: () => this.loading.set(false)
      });
    } else {
      const request = this.mapToCreateEmployeeRequest();

      this.api.create(request).subscribe({
        next: () => this.onSuccess(),
        error: () => this.loading.set(false)
      });
    }
  }

  private mapToCreateEmployeeWithUserRequest(): CreateEmployeeWithUserRequest {
    const raw = this.form.getRawValue();

    return {
      employee: {
        employeeNumber: raw.employee.employeeNumber!,
        firstName: raw.employee.firstName!,
        lastName: raw.employee.lastName!,
        email: raw.employee.email!,
        hireDate: parseDateOnly(raw.employee.hireDate!),
        departmentId: raw.employee.departmentId!,
      },
      user: {
        userName: raw.user.userName!,
        email: raw.user.email!,
        password: raw.user.password!,
      },
      role: raw.role!
    };
  }  

  private mapToCreateEmployeeRequest(): CreateEmployeeRequest {
    const raw = this.form.getRawValue();

    return {
      employeeNumber: raw.employee.employeeNumber!,
      firstName: raw.employee.firstName!,
      lastName: raw.employee.lastName!,
      email: raw.employee.email!,
      hireDate: parseDateOnly(raw.employee.hireDate!),
      departmentId: raw.employee.departmentId!,
    };
  }  

  private onSuccess() {
    this.loading.set(false);
    this.router.navigate(["/employees"], { state: { success: true }});
  }
}
